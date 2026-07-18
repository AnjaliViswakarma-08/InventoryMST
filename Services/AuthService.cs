using System.Security.Cryptography;
using InventoryMS.Data;
using InventoryMS.Data.Models;
using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.DTOs.Auth;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using InventoryMS.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IOtpRepository _otpRepository;
    private readonly IEmailService _emailService;

    // In-memory store for reset tokens (email -> hashedToken, expiresAtUtc)
    // In production, use a distributed cache like Redis
    private static readonly Dictionary<string, (string HashedToken, DateTime ExpiresAtUtc, string Purpose)> _resetTokens = new(StringComparer.OrdinalIgnoreCase);

    public AuthService(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        IPasswordHasher<User> passwordHasher,
        IOtpRepository otpRepository,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _otpRepository = otpRepository;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var (token, expiresAtUtc) = _tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.UserId,
            FullName = $"{user.Firstname} {user.Lastname}",
            Role = user.Role?.Name ?? string.Empty
        };
    }

    public async Task SignUpAsync(SignUpRequestDto dto, CancellationToken cancellationToken)
    {
        // Check if email already registered
        var existingUser = await _userRepository.EmailExistsAsync(dto.Email, null, cancellationToken);

        if (existingUser)
        {
            throw new ConflictException("An account with this email already exists. Please login instead.");
        }

        // Invalidate any previous OTPs for this email
        await _otpRepository.InvalidateAllForEmailAsync(dto.Email, "Join", cancellationToken);

        // Generate 6-digit OTP
        var otp = GenerateOtp();
        var otpHash = HashOtp(otp);

        var otpEntry = new OtpEntry
        {
            Email = dto.Email.ToLower(),
            CodeHash = otpHash,
            Purpose = "Join",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _otpRepository.AddAsync(otpEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send OTP via email
        await _emailService.SendOtpAsync(dto.Email, otp, "Join", cancellationToken);
    }

    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto, CancellationToken cancellationToken)
    {
        var otpEntry = await _otpRepository.GetLatestValidAsync(dto.Email, dto.Purpose, cancellationToken);

        if (otpEntry is null)
        {
            throw new UnauthorizedAccessException("OTP is invalid or has expired. Please request a new one.");
        }

        // Verify the OTP hash
        var inputHash = HashOtp(dto.Otp);
        if (!string.Equals(otpEntry.CodeHash, inputHash, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Invalid OTP code.");
        }

        // Mark OTP as used
        otpEntry.IsUsed = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate a short-lived reset token
        var resetToken = GenerateResetToken();
        var hashedResetToken = HashOtp(resetToken);

        lock (_resetTokens)
        {
            _resetTokens[dto.Email.ToLower()] = (hashedResetToken, DateTime.UtcNow.AddMinutes(10), dto.Purpose);
        }

        return new VerifyOtpResponseDto
        {
            Email = dto.Email,
            ResetToken = resetToken
        };
    }

    public async Task<AuthResponseDto> SetPasswordAsync(SetPasswordRequestDto dto, CancellationToken cancellationToken)
    {
        // Validate reset token
        ValidateResetToken(dto.Email, dto.ResetToken, "Join");

        // Ensure email not already registered (race condition guard)
        var existingUser = await _userRepository.EmailExistsAsync(dto.Email, null, cancellationToken);

        if (existingUser)
        {
            throw new ConflictException("An account with this email already exists.");
        }

        // Create user with ViewerStaff role by default
        var viewerRole = await _roleRepository.GetByNameAsync(RoleName.ViewerStaff, cancellationToken)
            ?? throw new InvalidOperationException("ViewerStaff role not found. Database may not be seeded.");

        var user = new User
        {
            Firstname = dto.FirstName,
            Lastname = dto.LastName,
            Age = dto.Age,
            Gender = dto.Gender,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email.ToLower(),
            RoleId = viewerRole.RoleId,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Consume the reset token
        ConsumeResetToken(dto.Email);

        // Reload with role for token generation
        await _userRepository.LoadRoleAsync(user, cancellationToken);

        // Auto-login: return JWT
        var (token, expiresAtUtc) = _tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.UserId,
            FullName = $"{user.Firstname} {user.Lastname}",
            Role = user.Role?.Name ?? string.Empty
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto dto, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);

        if (user is null)
        {
            // Don't reveal whether email exists — silently succeed
            return;
        }

        // Invalidate previous OTPs
        await _otpRepository.InvalidateAllForEmailAsync(dto.Email, "ForgotPassword", cancellationToken);

        // Generate and send OTP
        var otp = GenerateOtp();
        var otpHash = HashOtp(otp);

        var otpEntry = new OtpEntry
        {
            Email = dto.Email.ToLower(),
            CodeHash = otpHash,
            Purpose = "ForgotPassword",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _otpRepository.AddAsync(otpEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _emailService.SendOtpAsync(dto.Email, otp, "ForgotPassword", cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto dto, CancellationToken cancellationToken)
    {
        // Validate reset token
        ValidateResetToken(dto.Email, dto.ResetToken, "ForgotPassword");

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        // Update password
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Consume the reset token
        ConsumeResetToken(dto.Email);
    }

    // ──────────── Private helpers ────────────

    private static string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    private static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    private static string HashOtp(string otp)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(otp);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private static void ValidateResetToken(string email, string resetToken, string expectedPurpose)
    {
        lock (_resetTokens)
        {
            if (!_resetTokens.TryGetValue(email.ToLower(), out var stored))
            {
                throw new UnauthorizedAccessException("Invalid or expired reset token. Please verify OTP again.");
            }

            if (stored.ExpiresAtUtc < DateTime.UtcNow)
            {
                _resetTokens.Remove(email.ToLower());
                throw new UnauthorizedAccessException("Reset token has expired. Please verify OTP again.");
            }

            if (!string.Equals(stored.Purpose, expectedPurpose, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Invalid reset token purpose.");
            }

            var inputHash = HashOtp(resetToken);
            if (!string.Equals(stored.HashedToken, inputHash, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("Invalid reset token.");
            }
        }
    }

    private static void ConsumeResetToken(string email)
    {
        lock (_resetTokens)
        {
            _resetTokens.Remove(email.ToLower());
        }
    }
}

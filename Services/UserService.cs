using AutoMapper;
using InventoryMS.Data;
using InventoryMS.DTOs.Users;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using InventoryMS.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace InventoryMS.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRoleRepository roleRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserResponseDto>> GetAllAsync(int actingUserId, string actingRole, CancellationToken cancellationToken)
    {
        if (string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actingRole, RoleName.HR, StringComparison.OrdinalIgnoreCase))
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        var user = await _userRepository.GetByIdAsync(actingUserId, cancellationToken);
        if (user == null)
        {
            return new List<UserResponseDto>();
        }

        return new List<UserResponseDto> { _mapper.Map<UserResponseDto>(user) };
    }

    public async Task<UserResponseDto> GetByIdAsync(int userId, int actingUserId, string actingRole, CancellationToken cancellationToken)
    {
        if (!string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(actingRole, RoleName.HR, StringComparison.OrdinalIgnoreCase) &&
            userId != actingUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to view this user's data.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} was not found.");

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> CreateAsync(string actingRole, UserCreateDto dto, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email, null, cancellationToken))
        {
            throw new ConflictException("Email already exists.");
        }

        ValidateRolePermission(actingRole, dto.Role, "create");

        var user = _mapper.Map<User>(dto);
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        // Resolve role name -> role id
        var role = await _roleRepository.GetByNameAsync(dto.Role, cancellationToken);
        if (role is null)
        {
            throw new ArgumentException($"Role '{dto.Role}' does not exist.");
        }
        user.RoleId = role.RoleId;

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> UpdateAsync(int userId, int actingUserId, string actingRole, UserUpdateDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} was not found.");

        // Email update is totally denied for any user
        if (!string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Email update is not allowed.");
        }

        // Resolve the user's current role name
        var userRoleName = user.Role?.Name
            ?? (await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken))?.Name
            ?? string.Empty;

        if (userId == actingUserId)
        {
            // Self-edit: user can modify own profile but NOT their own role
            if (!string.IsNullOrWhiteSpace(dto.Role)
                && !string.Equals(dto.Role, userRoleName, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("You cannot change your own role.");
            }
            
            // Map all updated details
            _mapper.Map(dto, user);
            // Force role to remain the same
            var currentRole = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
            if (currentRole != null)
            {
                user.Role = currentRole;
            }
        }
        else
        {
            // Admin-edit: Owner/HR editing another user. ONLY allowed to modify the role.
            if (!string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(actingRole, RoleName.HR, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Only Owner and HR can edit other users.");
            }

            // Do NOT map other fields from DTO. Only update the role if it changed.
            if (!string.Equals(dto.Role, userRoleName, StringComparison.OrdinalIgnoreCase))
            {
                var newRole = await _roleRepository.GetByNameAsync(dto.Role, cancellationToken);
                if (newRole is null)
                {
                    throw new ArgumentException($"Role '{dto.Role}' does not exist.");
                }

                // HR can only update the role of Staff (AdminStaff, EditorStaff, ViewerStaff)
                if (string.Equals(actingRole, RoleName.HR, StringComparison.OrdinalIgnoreCase))
                {
                    bool isCurrentRoleStaff = string.Equals(userRoleName, RoleName.AdminStaff, StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(userRoleName, RoleName.EditorStaff, StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(userRoleName, RoleName.ViewerStaff, StringComparison.OrdinalIgnoreCase);

                    bool isNewRoleStaff = string.Equals(dto.Role, RoleName.AdminStaff, StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(dto.Role, RoleName.EditorStaff, StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(dto.Role, RoleName.ViewerStaff, StringComparison.OrdinalIgnoreCase);

                    if (!isCurrentRoleStaff || !isNewRoleStaff)
                    {
                        throw new UnauthorizedAccessException("HR can only update the role of Staff (AdminStaff, EditorStaff, ViewerStaff).");
                    }
                }

                user.RoleId = newRole.RoleId;
                user.Role = newRole;
            }
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Ensure user.Role navigation property is explicitly loaded/populated for DTO mapping
        if (user.Role == null)
        {
            await _userRepository.LoadRoleAsync(user, cancellationToken);
        }

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeleteAsync(int userId, int actingUserId, string actingRole, CancellationToken cancellationToken)
    {
        if (userId == actingUserId)
        {
            throw new UnauthorizedAccessException("You cannot delete your own account.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} was not found.");

        var userRoleName = user.Role?.Name
            ?? (await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken))?.Name
            ?? string.Empty;
        ValidateRolePermission(actingRole, userRoleName, "delete");

        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Validates that the acting user has permission to manage a user with the target role.
    /// Owner: can manage any role.
    /// HR: can manage any role except Owner.
    /// All others: cannot manage users.
    /// </summary>
    private static void ValidateRolePermission(string actingRole, string targetRole, string action)
    {
        if (string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase))
        {
            return; // Owner can do everything
        }

        if (string.Equals(actingRole, RoleName.HR, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(targetRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"HR cannot {action} Owner users.");
            }
            return; // HR can manage all non-Owner roles
        }

        throw new UnauthorizedAccessException($"Only Owner and HR can {action} users.");
    }
}

using AutoMapper;
using InventoryMS.Data;
using InventoryMS.DTOs.Users;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using InventoryMS.Repositories.Interfaces;
using InventoryMS.Models;
using InventoryMS.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace InventoryMS.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> GetByIdAsync(int userId, CancellationToken cancellationToken)
    {
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
        var role = await _dbContext.Roles.SingleOrDefaultAsync(r => r.Name == dto.Role, cancellationToken);
        if (role is null)
        {
            throw new ArgumentException($"Role '{dto.Role}' does not exist.");
        }
        user.RoleId = role.RoleId;

        await _userRepository.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> UpdateAsync(int userId, string actingRole, UserUpdateDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} was not found.");

        if (await _userRepository.EmailExistsAsync(dto.Email, userId, cancellationToken))
        {
            throw new ConflictException("Email already exists.");
        }

        // Ensure role navigation is available for permission checks
        var userRoleName = user.Role?.Name ?? (await _dbContext.Roles.FindAsync(new object?[] { user.RoleId }, cancellationToken))?.Name ?? string.Empty;
        ValidateRolePermission(actingRole, userRoleName, "update");
        ValidateRolePermission(actingRole, dto.Role, "update");

        _mapper.Map(dto, user);

        // Update role id if role name changed
        if (!string.Equals(dto.Role, userRoleName, StringComparison.OrdinalIgnoreCase))
        {
            var newRole = await _dbContext.Roles.SingleOrDefaultAsync(r => r.Name == dto.Role, cancellationToken);
            if (newRole is null)
            {
                throw new ArgumentException($"Role '{dto.Role}' does not exist.");
            }
            user.RoleId = newRole.RoleId;
        }
        _userRepository.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeleteAsync(int userId, string actingRole, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} was not found.");

        var userRoleName = user.Role?.Name ?? (await _dbContext.Roles.FindAsync(new object?[] { user.RoleId }, cancellationToken))?.Name ?? string.Empty;
        ValidateRolePermission(actingRole, userRoleName, "delete");

        _userRepository.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
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

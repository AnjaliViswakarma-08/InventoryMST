using AutoMapper;
using InventoryMS.Data;
using InventoryMS.DTOs.Users;
using InventoryMS.Helpers;
using InventoryMS.Interfaces;
using InventoryMS.Models;
using InventoryMS.Exceptions;
using Microsoft.AspNetCore.Identity;


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

        if(!string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase) && (string.Equals(dto.Role, RoleName.Owner, StringComparison.OrdinalIgnoreCase) || string.Equals(dto.Role, RoleName.Staff, StringComparison.OrdinalIgnoreCase) || string.Equals(dto.Role, RoleName.Admin, StringComparison.OrdinalIgnoreCase)))
        {
            throw new UnauthorizedAccessException("Only owners can create Owner, Staff, or Admin users");
        }

        var user = _mapper.Map<User>(dto);
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

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

        if(!string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase) && (string.Equals(dto.Role, RoleName.Owner, StringComparison.OrdinalIgnoreCase) || string.Equals(dto.Role, RoleName.Staff, StringComparison.OrdinalIgnoreCase) || string.Equals(dto.Role, RoleName.Admin, StringComparison.OrdinalIgnoreCase)))
        {
            throw new UnauthorizedAccessException("Only owners can update Owner, Staff, or Admin users");
        }


        _mapper.Map(dto, user);
        _userRepository.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeleteAsync(int userId, string actingRole, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} was not found.");

        if(!string.Equals(actingRole, RoleName.Owner, StringComparison.OrdinalIgnoreCase) && (string.Equals(user.Role, RoleName.Owner, StringComparison.OrdinalIgnoreCase) || string.Equals(user.Role, RoleName.Staff, StringComparison.OrdinalIgnoreCase) || string.Equals(user.Role, RoleName.Admin, StringComparison.OrdinalIgnoreCase)))
        {
            throw new UnauthorizedAccessException("Only owners can delete Owner, Staff, or Admin users");
        }

        _userRepository.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

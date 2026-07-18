using InventoryMS.DTOs.Users;

namespace InventoryMS.Services.Interfaces;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponseDto> GetByIdAsync(int userId, CancellationToken cancellationToken);

    Task<UserResponseDto> CreateAsync(string actingRole, UserCreateDto dto, CancellationToken cancellationToken);

    Task<UserResponseDto> UpdateAsync(int userId, int actingUserId, string actingRole, UserUpdateDto dto, CancellationToken cancellationToken);

    Task DeleteAsync(int userId, int actingUserId, string actingRole, CancellationToken cancellationToken);
}

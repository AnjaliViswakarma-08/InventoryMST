using InventoryMS.DTOs.Auth;

namespace InventoryMS.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken);
}

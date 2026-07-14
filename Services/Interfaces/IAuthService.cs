using InventoryMS.DTOs.Auth;

namespace InventoryMS.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken);
}

using InventoryMS.Data.Models;

namespace InventoryMS.Services.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) CreateToken(User user);
}

using InventoryMS.Models;

namespace InventoryMS.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) CreateToken(User user);
}

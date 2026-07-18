using InventoryMS.Data.Models;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken);
}

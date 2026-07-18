using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _dbContext;

    public RoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken) =>
        _dbContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower(), cancellationToken);

    public Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken) =>
        _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId, cancellationToken);
}

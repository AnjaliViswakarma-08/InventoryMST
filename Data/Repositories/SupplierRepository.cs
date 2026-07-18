using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _dbContext;

    public SupplierRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Suppliers.AsNoTracking().OrderBy(s => s.SupplierId).ToListAsync(cancellationToken);

    public Task<Supplier?> GetByIdAsync(int supplierId, CancellationToken cancellationToken) =>
        _dbContext.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == supplierId, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, int? excludeSupplierId, CancellationToken cancellationToken) =>
        _dbContext.Suppliers.AnyAsync(s =>
            s.Email.ToLower() == email.ToLower() && (!excludeSupplierId.HasValue || s.SupplierId != excludeSupplierId.Value),
            cancellationToken);

    public Task AddAsync(Supplier supplier, CancellationToken cancellationToken) =>
        _dbContext.Suppliers.AddAsync(supplier, cancellationToken).AsTask();

    public void Update(Supplier supplier) => _dbContext.Suppliers.Update(supplier);

    public void Remove(Supplier supplier) => _dbContext.Suppliers.Remove(supplier);
}

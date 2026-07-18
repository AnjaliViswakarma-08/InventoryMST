using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Supplier)
            .OrderBy(p => p.ProductId)
            .ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken) =>
        _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);

    public Task<Product?> GetByIdWithSupplierAsync(int productId, CancellationToken cancellationToken) =>
        _dbContext.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);

    public Task<bool> ExistsByNameAsync(string productName, int? excludeProductId, CancellationToken cancellationToken) =>
        _dbContext.Products.AnyAsync(p =>
            p.ProductName.ToLower() == productName.ToLower() && (!excludeProductId.HasValue || p.ProductId != excludeProductId.Value),
            cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken) =>
        _dbContext.Products.AddAsync(product, cancellationToken).AsTask();

    public void Update(Product product) => _dbContext.Products.Update(product);

    public void Remove(Product product) => _dbContext.Products.Remove(product);
}

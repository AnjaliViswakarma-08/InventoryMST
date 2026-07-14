using InventoryMS.Models;

namespace InventoryMS.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken);

    Task<Product?> GetByIdWithSupplierAsync(int productId, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string productName, int? excludeProductId, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    void Update(Product product);

    void Remove(Product product);
}

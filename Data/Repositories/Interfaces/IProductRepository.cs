using InventoryMS.Data.Models;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken);

    Task<Product?> GetByIdWithSupplierAsync(int productId, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string productName, int? excludeProductId, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    void Update(Product product);

    void Remove(Product product);

    Task<List<Product>> GetByIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken);
}

using InventoryMS.Data.Models;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken);

    Task<Supplier?> GetByIdAsync(int supplierId, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, int? excludeSupplierId, CancellationToken cancellationToken);

    Task AddAsync(Supplier supplier, CancellationToken cancellationToken);

    void Update(Supplier supplier);

    void Remove(Supplier supplier);
}

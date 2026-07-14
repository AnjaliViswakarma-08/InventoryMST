using InventoryMS.DTOs.Suppliers;

namespace InventoryMS.Services.Interfaces;

public interface ISupplierService
{
    Task<List<SupplierResponseDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<SupplierResponseDto> GetByIdAsync(int supplierId, CancellationToken cancellationToken);

    Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto, CancellationToken cancellationToken);

    Task<SupplierResponseDto> UpdateAsync(int supplierId, SupplierUpdateDto dto, CancellationToken cancellationToken);

    Task DeleteAsync(int supplierId, CancellationToken cancellationToken);
}

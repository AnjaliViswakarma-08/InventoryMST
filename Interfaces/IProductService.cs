using InventoryMS.DTOs.Products;

namespace InventoryMS.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProductResponseDto> GetByIdAsync(int productId, CancellationToken cancellationToken);

    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken cancellationToken);

    Task<ProductResponseDto> UpdateAsync(int productId, ProductUpdateDto dto, CancellationToken cancellationToken);

    Task DeleteAsync(int productId, CancellationToken cancellationToken);
}

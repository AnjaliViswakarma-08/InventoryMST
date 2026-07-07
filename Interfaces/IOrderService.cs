using InventoryMS.DTOs.Orders;

namespace InventoryMS.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<OrderResponseDto> GetByIdAsync(int orderId, CancellationToken cancellationToken);

    Task<OrderResponseDto> CreateAsync(OrderCreateDto dto, CancellationToken cancellationToken);

    Task<OrderResponseDto> UpdateAsync(int orderId, OrderCreateDto dto, CancellationToken cancellationToken);

    Task DeleteAsync(int orderId, CancellationToken cancellationToken);
}

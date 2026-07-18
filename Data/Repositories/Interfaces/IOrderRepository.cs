using InventoryMS.Data.Models;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken);

    Task<Order?> GetByIdWithItemsAsync(int orderId, CancellationToken cancellationToken);

    Task AddAsync(Order order, CancellationToken cancellationToken);

    void Update(Order order);

    void Remove(Order order);

    void RemoveOrderItems(IEnumerable<OrderItem> items);
}

using InventoryMS.Data.Repositories.Interfaces;
using InventoryMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .OrderBy(o => o.OrderId)
            .ToListAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken) =>
        _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

    public Task<Order?> GetByIdWithItemsAsync(int orderId, CancellationToken cancellationToken) =>
        _dbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

    public Task AddAsync(Order order, CancellationToken cancellationToken) =>
        _dbContext.Orders.AddAsync(order, cancellationToken).AsTask();

    public void Update(Order order) => _dbContext.Orders.Update(order);

    public void Remove(Order order) => _dbContext.Orders.Remove(order);

    public void RemoveOrderItems(IEnumerable<OrderItem> items) => _dbContext.OrderItems.RemoveRange(items);
}

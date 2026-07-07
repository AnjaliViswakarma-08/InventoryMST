using AutoMapper;
using InventoryMS.Data;
using InventoryMS.DTOs.Orders;
using InventoryMS.Helpers;
using InventoryMS.Interfaces;
using InventoryMS.Models;
using InventoryMS.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;

    public OrderService(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IMapper mapper,
        AppDbContext dbContext)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<List<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetByIdAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order {orderId} was not found.");

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> CreateAsync(OrderCreateDto dto, CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(dto.UserId, cancellationToken);
        ValidateItems(dto.Items);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var order = new Order
        {
            UserId = dto.UserId,
            OrderDate = DateTime.UtcNow
        };

        await HydrateOrderItemsAsync(order, dto.Items, cancellationToken);
        await _orderRepository.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetByIdAsync(order.OrderId, cancellationToken);
    }

    public async Task<OrderResponseDto> UpdateAsync(int orderId, OrderCreateDto dto, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(existing => existing.OrderItems)
            .FirstOrDefaultAsync(existing => existing.OrderId == orderId, cancellationToken)
            ?? throw new NotFoundException($"Order {orderId} was not found.");

        await EnsureUserExistsAsync(dto.UserId, cancellationToken);
        ValidateItems(dto.Items);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        await RestoreInventoryAsync(order.OrderItems, cancellationToken);
        _dbContext.OrderItems.RemoveRange(order.OrderItems);
        order.OrderItems.Clear();
        order.UserId = dto.UserId;
        order.OrderDate = DateTime.UtcNow;

        await HydrateOrderItemsAsync(order, dto.Items, cancellationToken);
        _orderRepository.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetByIdAsync(orderId, cancellationToken);
    }

    public async Task DeleteAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(existing => existing.OrderItems)
            .FirstOrDefaultAsync(existing => existing.OrderId == orderId, cancellationToken)
            ?? throw new NotFoundException($"Order {orderId} was not found.");

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        await RestoreInventoryAsync(order.OrderItems, cancellationToken);
        _orderRepository.Remove(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task EnsureUserExistsAsync(int userId, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(userId, cancellationToken) is null)
        {
            throw new NotFoundException($"User {userId} was not found.");
        }
    }

    private static void ValidateItems(IEnumerable<OrderItemCreateDto> items)
    {
        if (!items.Any())
        {
            throw new InvalidOperationException("At least one order item is required.");
        }

        if (items.Any(item => item.Quantity <= 0))
        {
            throw new InvalidOperationException("Each order item quantity must be greater than zero.");
        }
    }

    private async Task HydrateOrderItemsAsync(Order order, IEnumerable<OrderItemCreateDto> items, CancellationToken cancellationToken)
    {
        var groupedItems = items
            .GroupBy(item => item.ProductId)
            .Select(group => new OrderItemCreateDto { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .ToList();

        var productIds = groupedItems.Select(item => item.ProductId).ToArray();
        var products = await _dbContext.Products
            .Where(product => productIds.Contains(product.ProductId))
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Length)
        {
            throw new NotFoundException("One or more products were not found.");
        }

        foreach (var item in groupedItems)
        {
            var product = products.First(product => product.ProductId == item.ProductId);
            if (product.QuantityInStock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.ProductName}.");
            }

            product.QuantityInStock -= item.Quantity;
            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.ProductPrice,
                TotalPrice = product.ProductPrice * item.Quantity
            });
        }

        order.TotalAmount = order.OrderItems.Sum(orderItem => orderItem.TotalPrice);
    }

    private async Task RestoreInventoryAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken)
    {
        var productIds = orderItems.Select(orderItem => orderItem.ProductId).Distinct().ToArray();
        var products = await _dbContext.Products
            .Where(product => productIds.Contains(product.ProductId))
            .ToListAsync(cancellationToken);

        foreach (var orderItem in orderItems)
        {
            var product = products.First(product => product.ProductId == orderItem.ProductId);
            product.QuantityInStock += orderItem.Quantity;
        }
    }
}

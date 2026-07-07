namespace InventoryMS.DTOs.Orders;

public sealed class OrderCreateDto
{
    public int UserId { get; set; }

    public List<OrderItemCreateDto> Items { get; set; } = new();
}

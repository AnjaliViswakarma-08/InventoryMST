namespace InventoryMS.DTOs.Orders;

public sealed class OrderItemCreateDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}

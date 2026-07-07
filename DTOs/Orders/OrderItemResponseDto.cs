namespace InventoryMS.DTOs.Orders;

public sealed class OrderItemResponseDto
{
    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}

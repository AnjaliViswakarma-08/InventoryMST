namespace InventoryMS.DTOs.Orders;

public sealed class OrderResponseDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public List<OrderItemResponseDto> Items { get; set; } = new();
}

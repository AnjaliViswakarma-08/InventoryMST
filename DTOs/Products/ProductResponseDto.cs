namespace InventoryMS.DTOs.Products;

public sealed class ProductResponseDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}

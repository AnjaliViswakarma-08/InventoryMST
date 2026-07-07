namespace InventoryMS.DTOs.Products;

public sealed class ProductCreateDto
{
    public string ProductName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public int SupplierId { get; set; }
}

namespace InventoryMS.Data.Models;

public sealed class Product
{
    public int ProductId { get; set;}
    public string ProductName { get; set;} = string.Empty;
    public string ProductDesc { get; set;} = string.Empty;
    public decimal ProductPrice { get; set;}
    public int QuantityInStock { get; set;}
    public int SupplierId { get; set;}
    public DateTime CreatedAt { get; set;}
    public Supplier? Supplier { get; set;}
    public ICollection<OrderItem> OrderItems { get; set;} = new List<OrderItem>();

}

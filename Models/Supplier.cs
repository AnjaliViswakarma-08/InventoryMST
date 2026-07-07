namespace InventoryMS.Models;

public sealed class Supplier
{
    public int SupplierId {get; set;}
    public string SupplierName { get; set;} =string.Empty;
    public string ContactPerson { get; set;} = string.Empty;
    public string Phone { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string Address { get; set;} = string.Empty;
    public ICollection<Product> Products { get; set;} = new List<Product>();
}
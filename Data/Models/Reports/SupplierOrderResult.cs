namespace InventoryMS.Data.Models.Reports;

public sealed class SupplierOrderResult
{
    public string SupplierName { get; set;} = string.Empty;
    public int TotalQuantityOrdered { get; set;}
    public decimal TotalOrderValue { get; set;}
}

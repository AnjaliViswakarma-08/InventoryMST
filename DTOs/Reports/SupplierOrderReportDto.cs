namespace InventoryMS.DTOs.Reports;

public sealed class SupplierOrderReportDto
{
    public string SupplierName { get; set; } = string.Empty;

    public int TotalQuantityOrdered { get; set; }

    public decimal TotalOrderValue { get; set; }
}

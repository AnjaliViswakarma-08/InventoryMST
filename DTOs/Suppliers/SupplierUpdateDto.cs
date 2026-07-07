namespace InventoryMS.DTOs.Suppliers;

public sealed class SupplierUpdateDto
{
    public string SupplierName { get; set; } = string.Empty;

    public string ContactPerson { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}

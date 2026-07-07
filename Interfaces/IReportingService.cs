using InventoryMS.DTOs.Reports;

namespace InventoryMS.Interfaces;

public interface IReportingService
{
    Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken);
}

using InventoryMS.DTOs.Reports;

namespace InventoryMS.Services.Interfaces;

public interface IReportingService
{
    Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken);
}

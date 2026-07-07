using InventoryMS.DTOs.Reports;

namespace InventoryMS.Interfaces;

public interface IReportingRepository
{
    Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken);
}

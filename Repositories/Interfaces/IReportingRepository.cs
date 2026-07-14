using InventoryMS.DTOs.Reports;

namespace InventoryMS.Repositories.Interfaces;

public interface IReportingRepository
{
    Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken);
}

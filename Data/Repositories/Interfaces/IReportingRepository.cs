using InventoryMS.DTOs.Reports;

namespace InventoryMS.Data.Repositories.Interfaces;

public interface IReportingRepository
{
    Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken);
}

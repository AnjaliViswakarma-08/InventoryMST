using InventoryMS.DTOs.Reports;
using InventoryMS.Interfaces;

namespace InventoryMS.Services;

public sealed class ReportingService : IReportingService
{
    private readonly IReportingRepository _reportingRepository;

    public ReportingService(IReportingRepository reportingRepository)
    {
        _reportingRepository = reportingRepository;
    }

    public Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken) =>
        _reportingRepository.GetSupplierOrderReportAsync(cancellationToken);
}

using InventoryMS.DTOs.Reports;
using InventoryMS.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryMS.Data.Repositories;

public sealed class ReportingRepository : IReportingRepository
{
    private readonly AppDbContext _dbContext;

    public ReportingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SupplierOrderReportDto>> GetSupplierOrderReportAsync(CancellationToken cancellationToken)
    {
        var rows = await _dbContext.SupplierOrderResults
            .FromSqlRaw("EXEC dbo.sp_GetSupplierOrderReport")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows.Select(row => new SupplierOrderReportDto
        {
            SupplierName = row.SupplierName,
            TotalQuantityOrdered = row.TotalQuantityOrdered,
            TotalOrderValue = row.TotalOrderValue
        }).ToList();
    }
}

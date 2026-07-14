using InventoryMS.DTOs.Reports;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMS.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Owner")]
public sealed class ReportController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    /// <summary>Gets the supplier order summary report.</summary>
    [HttpGet("supplier-orders")]
    [ProducesResponseType(typeof(ApiResponse<List<SupplierOrderReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SupplierOrderReportDto>>>> GetSupplierOrders(CancellationToken cancellationToken)
    {
        var report = await _reportingService.GetSupplierOrderReportAsync(cancellationToken);
        return Ok(ApiResponse<List<SupplierOrderReportDto>>.Ok(report));
    }
}

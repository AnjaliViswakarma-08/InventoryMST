using InventoryMS.DTOs.Suppliers;
using InventoryMS.Helpers;
using InventoryMS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMS.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public sealed class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<SupplierResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SupplierResponseDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var suppliers = await _supplierService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<SupplierResponseDto>>.Ok(suppliers));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<SupplierResponseDto>.Ok(supplier));
    }

    [Authorize(Roles = "Owner,Staff")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> Create([FromBody] SupplierCreateDto dto, CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, ApiResponse<SupplierResponseDto>.Ok(supplier, "Supplier created successfully."));
    }

    [Authorize(Roles = "Owner,Staff")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SupplierResponseDto>>> Update(int id, [FromBody] SupplierUpdateDto dto, CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.UpdateAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<SupplierResponseDto>.Ok(supplier, "Supplier updated successfully."));
    }

    [Authorize(Roles = "Owner,Staff")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _supplierService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "Supplier deleted successfully."));
    }
}

using InventoryMS.DTOs.Products;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMS.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public sealed class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Gets all products.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<ProductResponseDto>>.Ok(products));
    }

    /// <summary>Gets a product by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ProductResponseDto>.Ok(product));
    }

    /// <summary>Creates a new product.</summary>
    [Authorize(Roles = "Owner,AdminStaff,EditorStaff")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] ProductCreateDto dto, CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, ApiResponse<ProductResponseDto>.Ok(product, "Product created successfully."));
    }

    /// <summary>Updates a product.</summary>
    [Authorize(Roles = "Owner,AdminStaff,EditorStaff")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(int id, [FromBody] ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        var product = await _productService.UpdateAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<ProductResponseDto>.Ok(product, "Product updated successfully."));
    }

    /// <summary>Deletes a product.</summary>
    [Authorize(Roles = "Owner,AdminStaff")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "Product deleted successfully."));
    }
}

using InventoryMS.DTOs.Orders;
using InventoryMS.Helpers;
using InventoryMS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMS.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Gets all orders.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<OrderResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OrderResponseDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<OrderResponseDto>>.Ok(orders));
    }

    /// <summary>Gets an order by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<OrderResponseDto>.Ok(order));
    }

    /// <summary>Creates a new order.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Create([FromBody] OrderCreateDto dto, CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, ApiResponse<OrderResponseDto>.Ok(order, "Order created successfully."));
    }

    /// <summary>Updates an order.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Update(int id, [FromBody] OrderCreateDto dto, CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<OrderResponseDto>.Ok(order, "Order updated successfully."));
    }

    /// <summary>Deletes an order.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _orderService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "Order deleted successfully."));
    }
}

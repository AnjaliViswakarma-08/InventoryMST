using InventoryMS.Helpers;
using InventoryMS.DTOs.Users;
using InventoryMS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryMS.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Get All Users</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<UserResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<UserResponseDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<UserResponseDto>>.Ok(users));
    }

    /// <summary>Get User By Id</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status200OK)] 
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<UserResponseDto>.Ok(user));
    }

    /// <summary>Create User</summary>
    [Authorize(Roles = "Owner")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> Create([FromBody] UserCreateDto dto, CancellationToken cancellationToken)
    {
        var actingRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var user = await _userService.CreateAsync(actingRole, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, ApiResponse<UserResponseDto>.Ok(user, "User created successfully"));
    }

    /// <summary>Updates a user.</summary>
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> Update(int id, [FromBody] UserUpdateDto dto, CancellationToken cancellationToken)
    {
        var actingRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var user = await _userService.UpdateAsync(id, actingRole, dto, cancellationToken);
        return Ok(ApiResponse<UserResponseDto>.Ok(user, "User updated successfully"));
    }

    /// <summary>Deletes a user.</summary>
    [Authorize(Roles = "Owner")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id, CancellationToken cancellationToken)
    {
        var actingRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        await _userService.DeleteAsync(id, actingRole, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "User deleted successfully"));
    }
}
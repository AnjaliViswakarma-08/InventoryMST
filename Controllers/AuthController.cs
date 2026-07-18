using InventoryMS.DTOs.Auth;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMS.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Authenticates a user and returns a JWT token with their role claim.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful."));
    }

    /// <summary>Initiates new user registration — sends an OTP to the provided email.</summary>
    [AllowAnonymous]
    [HttpPost("signup")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> SignUp([FromBody] SignUpRequestDto dto, CancellationToken cancellationToken)
    {
        await _authService.SignUpAsync(dto, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "OTP has been sent to your email. It is valid for 5 minutes."));
    }

    /// <summary>Verifies an OTP code and returns a short-lived reset token.</summary>
    [AllowAnonymous]
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(ApiResponse<VerifyOtpResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<VerifyOtpResponseDto>>> VerifyOtp([FromBody] VerifyOtpRequestDto dto, CancellationToken cancellationToken)
    {
        var response = await _authService.VerifyOtpAsync(dto, cancellationToken);
        return Ok(ApiResponse<VerifyOtpResponseDto>.Ok(response, "OTP verified successfully."));
    }

    /// <summary>Completes registration — sets the password and creates the user account. Returns JWT for auto-login.</summary>
    [AllowAnonymous]
    [HttpPost("set-password")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> SetPassword([FromBody] SetPasswordRequestDto dto, CancellationToken cancellationToken)
    {
        var response = await _authService.SetPasswordAsync(dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(response, "Account created successfully. You are now logged in."));
    }

    /// <summary>Initiates forgot-password flow — sends an OTP to the registered email.</summary>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequestDto dto, CancellationToken cancellationToken)
    {
        await _authService.ForgotPasswordAsync(dto, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "If an account with this email exists, an OTP has been sent."));
    }

    /// <summary>Resets the password after forgot-password OTP verification.</summary>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordRequestDto dto, CancellationToken cancellationToken)
    {
        await _authService.ResetPasswordAsync(dto, cancellationToken);
        return Ok(ApiResponse<string>.Ok(null, "Password has been reset successfully. You can now login with your new password."));
    }

    /// <summary>Logs out the current user (client-side token removal).</summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<string>> Logout()
    {
        // JWT is stateless — logout is handled client-side by discarding the token.
        // This endpoint exists for API completeness and can be extended with token blacklisting.
        return Ok(ApiResponse<string>.Ok(null, "Logged out successfully."));
    }
}

namespace InventoryMS.DTOs.Auth;

/// <summary>
/// Request to reset password after forgot-password OTP verification.
/// </summary>
public sealed class ResetPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

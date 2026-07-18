namespace InventoryMS.DTOs.Auth;

/// <summary>
/// Request to initiate forgot-password flow. OTP will be sent to the email.
/// </summary>
public sealed class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}

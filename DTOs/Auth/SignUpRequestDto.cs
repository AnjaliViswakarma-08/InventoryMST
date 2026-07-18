namespace InventoryMS.DTOs.Auth;

/// <summary>
/// Request to initiate a new user registration. An OTP will be sent to the provided email.
/// </summary>
public sealed class SignUpRequestDto
{
    public string Email { get; set; } = string.Empty;
}

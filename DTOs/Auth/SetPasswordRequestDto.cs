namespace InventoryMS.DTOs.Auth;

/// <summary>
/// Request to set the password after a successful OTP-verified sign-up.
/// Also carries basic profile info for account creation.
/// </summary>
public sealed class SetPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Profile fields for new registration
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

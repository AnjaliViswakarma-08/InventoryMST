namespace InventoryMS.DTOs.Auth;

/// <summary>
/// Request to verify an OTP code.
/// </summary>
public sealed class VerifyOtpRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;

    /// <summary>
    /// Purpose of OTP verification: "Join" or "ForgotPassword"
    /// </summary>
    public string Purpose { get; set; } = string.Empty;
}

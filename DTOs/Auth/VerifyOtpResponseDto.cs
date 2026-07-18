namespace InventoryMS.DTOs.Auth;

/// <summary>
/// Response after successful OTP verification. Contains a short-lived reset token.
/// </summary>
public sealed class VerifyOtpResponseDto
{
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// A short-lived token authorising the password set/reset operation.
    /// </summary>
    public string ResetToken { get; set; } = string.Empty;
}

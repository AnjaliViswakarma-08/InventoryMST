namespace InventoryMS.Data.Models;

public sealed class OtpEntry
{
    public int OtpEntryId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsUsed { get; set; }
    /// <summary>
    /// Purpose of the OTP: "Join" or "ForgotPassword"
    /// </summary>
    public string Purpose { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

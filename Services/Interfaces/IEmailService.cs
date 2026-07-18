namespace InventoryMS.Services.Interfaces;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp, string purpose, CancellationToken cancellationToken);
}

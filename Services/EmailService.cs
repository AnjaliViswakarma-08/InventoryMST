using System.Net;
using System.Net.Mail;
using InventoryMS.Helpers;
using InventoryMS.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InventoryMS.Services;

public sealed class EmailService : IEmailService
{
    private readonly SmtpOptions _smtp;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpOptions> smtp, ILogger<EmailService> logger)
    {
        _smtp = smtp.Value;
        _logger = logger;
    }

    public async Task SendOtpAsync(string toEmail, string otp, string purpose, CancellationToken cancellationToken)
    {
        var subject = purpose switch
        {
            "Join" => "Welcome to GoDamm Warehouse — Your OTP",
            "ForgotPassword" => "GoDamm Warehouse — Password Reset OTP",
            _ => "GoDamm Warehouse — Your OTP"
        };

        var body = $"""
            <html>
            <body style="font-family: 'Segoe UI', Arial, sans-serif; background-color: #f4f4f4; padding: 20px;">
                <div style="max-width: 480px; margin: 0 auto; background: #ffffff; border-radius: 12px; padding: 32px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);">
                    <h2 style="color: #1a1a2e; margin-bottom: 8px;">🔐 Your Verification Code</h2>
                    <p style="color: #555; font-size: 14px; margin-bottom: 24px;">
                        Use the code below to {(purpose == "Join" ? "complete your registration" : "reset your password")}:
                    </p>
                    <div style="text-align: center; margin: 24px 0;">
                        <span style="display: inline-block; font-size: 32px; font-weight: 700; letter-spacing: 8px; color: #1a1a2e; background: #e8f0fe; padding: 16px 32px; border-radius: 8px;">
                            {otp}
                        </span>
                    </div>
                    <p style="color: #888; font-size: 12px; text-align: center;">
                        This code expires in <strong>5 minutes</strong>. Do not share it with anyone.
                    </p>
                    <hr style="border: none; border-top: 1px solid #eee; margin: 24px 0;" />
                    <p style="color: #aaa; font-size: 11px; text-align: center;">
                        GoDamm Warehouse — Inventory Management System
                    </p>
                </div>
            </body>
            </html>
            """;

        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            EnableSsl = true
        };

        var message = new MailMessage(_smtp.FromEmail, toEmail, subject, body)
        {
            IsBodyHtml = true
        };

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("OTP email sent to {Email} for purpose {Purpose}", toEmail, purpose);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
            throw new InvalidOperationException("Failed to send OTP email. Please try again later.");
        }
    }
}

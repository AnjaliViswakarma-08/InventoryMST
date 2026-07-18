using InventoryMS.DTOs.Auth;

namespace InventoryMS.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken);

    /// <summary>Sign-up: sends an OTP to the email for new registration.</summary>
    Task SignUpAsync(SignUpRequestDto dto, CancellationToken cancellationToken);

    /// <summary>Verifies an OTP and returns a short-lived reset token.</summary>
    Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto, CancellationToken cancellationToken);

    /// <summary>Completes registration — sets password and creates the user account.</summary>
    Task<AuthResponseDto> SetPasswordAsync(SetPasswordRequestDto dto, CancellationToken cancellationToken);

    /// <summary>Forgot password: sends an OTP to the registered email.</summary>
    Task ForgotPasswordAsync(ForgotPasswordRequestDto dto, CancellationToken cancellationToken);

    /// <summary>Resets the password after forgot-password OTP verification.</summary>
    Task ResetPasswordAsync(ResetPasswordRequestDto dto, CancellationToken cancellationToken);
}

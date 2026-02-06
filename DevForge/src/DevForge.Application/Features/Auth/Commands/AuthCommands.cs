using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record RegisterCommand(
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        string? PhoneNumber = null
    ) : IRequest<Result<AuthResponse>>;

    public record LoginCommand(
        string UsernameOrEmail,
        string Password,
        bool RememberMe = false,
        string? IpAddress = null
    ) : IRequest<Result<AuthResponse>>;

    public record RefreshTokenCommand(
        string RefreshToken,
        string? IpAddress = null
    ) : IRequest<Result<AuthResponse>>;

    public record LogoutCommand(
        string RefreshToken,
        string? IpAddress = null
    ) : IRequest<Result>;

    public record ChangePasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest<Result>;

    public record ForgotPasswordCommand(
        string Email
    ) : IRequest<Result>;

    public record ResetPasswordCommand(
        string Email,
        string Token,
        string Password,
        string ConfirmPassword
    ) : IRequest<Result>;

    public record ConfirmEmailCommand(
        string Email,
        string Token
    ) : IRequest<Result>;

    public record ResendEmailConfirmationCommand(
        string Email
    ) : IRequest<Result>;

    public record EnableTwoFactorCommand(
        Guid UserId,
        string Password
    ) : IRequest<Result<TwoFactorSetupResponse>>;

    public record DisableTwoFactorCommand(
        Guid UserId,
        string Password
    ) : IRequest<Result>;

    public record VerifyTwoFactorCommand(
        Guid UserId,
        string Code
    ) : IRequest<Result>;
}

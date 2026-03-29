using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DevForge.Application.Features.Auth.DTOs
{
    /// <summary>
    /// User registration request
    /// </summary>
    public record RegisterRequest(
        /// <summary>
        /// Username (3-50 characters, alphanumeric with underscore/hyphen)
        /// </summary>
        /// <example>johndoe</example>
        [Required]
        string Username,
        
        /// <summary>
        /// Email address
        /// </summary>
        /// <example>john@example.com</example>
        [Required]
        [EmailAddress]
        string Email,
        
        /// <summary>
        /// Password (min 8 chars, uppercase, lowercase, digit, special char)
        /// </summary>
        /// <example>SecureP@ss123</example>
        [Required]
        string Password,
        
        /// <summary>
        /// Confirm password (must match password)
        /// </summary>
        /// <example>SecureP@ss123</example>
        [Required]
        string ConfirmPassword,
        
        /// <summary>
        /// Phone number (optional, international format)
        /// </summary>
        /// <example>+1234567890</example>
        string? PhoneNumber = null
    );

    /// <summary>
    /// Login request
    /// </summary>
    public record LoginRequest(
        /// <summary>
        /// Username or email address
        /// </summary>
        /// <example>admin@devforge.com</example>
        [Required]
        string UsernameOrEmail,
        
        /// <summary>
        /// Password
        /// </summary>
        /// <example>Admin@123456</example>
        [Required]
        string Password,
        
        /// <summary>
        /// Remember me (extends refresh token to 30 days)
        /// </summary>
        /// <example>true</example>
        [DefaultValue(false)]
        bool RememberMe = false
    );

    /// <summary>
    /// Refresh token request
    /// </summary>
    public record RefreshTokenRequest(
        /// <summary>
        /// Refresh token from login response
        /// </summary>
        /// <example>CfDJ8KZqM2...</example>
        [Required]
        string RefreshToken
    );

    /// <summary>
    /// Change password request
    /// </summary>
    public record ChangePasswordRequest(
        /// <summary>
        /// Current password for verification
        /// </summary>
        /// <example>OldP@ss123</example>
        [Required]
        string CurrentPassword,
        
        /// <summary>
        /// New password
        /// </summary>
        /// <example>NewSecureP@ss456</example>
        [Required]
        string NewPassword,
        
        /// <summary>
        /// Confirm new password
        /// </summary>
        /// <example>NewSecureP@ss456</example>
        [Required]
        string ConfirmPassword
    );

    /// <summary>
    /// Forgot password request
    /// </summary>
    public record ForgotPasswordRequest(
        /// <summary>
        /// Email address to send reset link
        /// </summary>
        /// <example>user@example.com</example>
        [Required]
        [EmailAddress]
        string Email
    );

    /// <summary>
    /// Reset password request
    /// </summary>
    public record ResetPasswordRequest(
        /// <summary>
        /// Email address
        /// </summary>
        /// <example>user@example.com</example>
        [Required]
        [EmailAddress]
        string Email,
        
        /// <summary>
        /// Reset token from email
        /// </summary>
        /// <example>abc123xyz...</example>
        [Required]
        string Token,
        
        /// <summary>
        /// New password
        /// </summary>
        /// <example>NewSecureP@ss123</example>
        [Required]
        string Password,
        
        /// <summary>
        /// Confirm new password
        /// </summary>
        /// <example>NewSecureP@ss123</example>
        [Required]
        string ConfirmPassword
    );

    /// <summary>
    /// Email confirmation request
    /// </summary>
    public record ConfirmEmailRequest(
        /// <summary>
        /// Email address to confirm
        /// </summary>
        /// <example>user@example.com</example>
        [Required]
        [EmailAddress]
        string Email,
        
        /// <summary>
        /// Confirmation token from email
        /// </summary>
        /// <example>xyz789abc...</example>
        [Required]
        string Token
    );

    /// <summary>
    /// Enable two-factor authentication request
    /// </summary>
    public record EnableTwoFactorRequest(
        /// <summary>
        /// Current password for verification
        /// </summary>
        /// <example>YourCurrentPassword</example>
        [Required]
        string Password
    );

    /// <summary>
    /// Verify two-factor authentication request
    /// </summary>
    public record VerifyTwoFactorRequest(
        /// <summary>
        /// 6-digit TOTP code from authenticator app
        /// </summary>
        /// <example>123456</example>
        [Required]
        [StringLength(6, MinimumLength = 6)]
        string Code
    );

    /// <summary>
    /// Authentication response with tokens
    /// </summary>
    public record AuthResponse(
        /// <summary>
        /// JWT access token (valid for 1 hour)
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        string AccessToken,
        
        /// <summary>
        /// Refresh token (valid for 7-30 days)
        /// </summary>
        /// <example>CfDJ8KZqM2...</example>
        string RefreshToken,
        
        /// <summary>
        /// Access token expiration time (UTC)
        /// </summary>
        /// <example>2024-02-04T12:00:00Z</example>
        DateTime ExpiresAt,
        
        /// <summary>
        /// Authenticated user details
        /// </summary>
        UserDto User
    );

    /// <summary>
    /// User information
    /// </summary>
    public record UserDto(
        /// <summary>
        /// User unique identifier
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        Guid Id,
        
        /// <summary>
        /// Username
        /// </summary>
        /// <example>johndoe</example>
        string Username,
        
        /// <summary>
        /// Email address
        /// </summary>
        /// <example>john@example.com</example>
        string Email,
        
        /// <summary>
        /// Phone number
        /// </summary>
        /// <example>+1234567890</example>
        string? PhoneNumber,
        
        /// <summary>
        /// Account active status
        /// </summary>
        /// <example>true</example>
        bool IsActive,
        
        /// <summary>
        /// Email confirmation status
        /// </summary>
        /// <example>true</example>
        bool EmailConfirmed,
        
        /// <summary>
        /// Phone number confirmation status
        /// </summary>
        /// <example>false</example>
        bool PhoneNumberConfirmed,
        
        /// <summary>
        /// Two-factor authentication status
        /// </summary>
        /// <example>false</example>
        bool TwoFactorEnabled,
        
        /// <summary>
        /// Account creation date (UTC)
        /// </summary>
        /// <example>2024-01-01T10:00:00Z</example>
        DateTime CreatedAt,
        
        /// <summary>
        /// Last login date (UTC)
        /// </summary>
        /// <example>2024-02-04T08:30:00Z</example>
        DateTime? LastLoginAt,
        
        /// <summary>
        /// Assigned roles
        /// </summary>
        /// <example>["Administrator", "User"]</example>
        List<string> Roles,
        
        /// <summary>
        /// Granted permissions
        /// </summary>
        /// <example>["users.read", "users.create", "users.update"]</example>
        List<string> Permissions
    );

    /// <summary>
    /// Two-factor authentication setup response
    /// </summary>
    public record TwoFactorSetupResponse(
        /// <summary>
        /// Secret key for manual entry in authenticator app
        /// </summary>
        /// <example>JBSWY3DPEHPK3PXP</example>
        string SecretKey,
        
        /// <summary>
        /// QR code URI for scanning with authenticator app
        /// </summary>
        /// <example>otpauth://totp/DevForge:user@example.com?secret=secret_code&issuer=DevForge</example>
        string QrCodeUri
    );
}

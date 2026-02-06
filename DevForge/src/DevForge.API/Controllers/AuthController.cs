using DevForge.Application.Features.Auth.Commands;
using DevForge.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevForge.API.Controllers
{
    /// <summary>
    /// Authentication and authorization endpoints
    /// </summary>
    /// <remarks>
    /// Provides comprehensive authentication features including:
    /// - User registration and login
    /// - JWT token management (access and refresh tokens)
    /// - Password management (change, reset)
    /// - Email confirmation
    /// - Two-factor authentication (TOTP)
    /// - Account security features
    /// </remarks>
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController
        /// </summary>
        /// <param name="mediator">MediatR instance for CQRS pattern</param>
        /// <param name="logger">Logger instance</param>
        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/register
        ///     {
        ///        "username": "johndoe",
        ///        "email": "john@example.com",
        ///        "password": "SecureP@ss123",
        ///        "confirmPassword": "SecureP@ss123",
        ///        "phoneNumber": "+1234567890"
        ///     }
        /// 
        /// Password requirements:
        /// - Minimum 8 characters
        /// - At least one uppercase letter
        /// - At least one lowercase letter
        /// - At least one digit
        /// - At least one special character
        /// </remarks>
        /// <param name="request">User registration details</param>
        /// <returns>Authentication response with tokens and user information</returns>
        /// <response code="201">User successfully registered</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="409">Username or email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("User registration attempt for email: {Email}", request.Email);
            var command = new RegisterCommand(
                request.Username,
                request.Email,
                request.Password,
                request.ConfirmPassword,
                request.PhoneNumber
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Error.Type == DevForge.Application.Common.Models.ErrorType.Conflict)
                {
                    _logger.LogWarning("Registration failed - {ErrorCode}: {ErrorMessage}", result.Error.Code, result.Error.Message);
                    return Conflict(new { error = result.Error.Message, code = result.Error.Code });
                }

                _logger.LogWarning("Registration failed for {Email}: {ErrorMessage}", request.Email, result.Error.Message);
                return BadRequest(new { error = result.Error.Message, code = result.Error.Code, validationErrors = result.ValidationErrors });
            }

            _logger.LogInformation("User registered successfully: {UserId}", result.Data!.User.Id);
            return CreatedAtAction(nameof(Register), new { id = result.Data!.User.Id }, result.Data);
        }

        /// <summary>
        /// Authenticate user and obtain access tokens
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/login
        ///     {
        ///        "usernameOrEmail": "admin@devforge.com",
        ///        "password": "Admin@123456",
        ///        "rememberMe": true
        ///     }
        /// 
        /// Login can be done using either username or email.
        /// Remember me extends refresh token expiration to 30 days (default: 7 days).
        /// </remarks>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication tokens and user details</returns>
        /// <response code="200">Successfully authenticated</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Invalid credentials or account locked</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {UsernameOrEmail}", request.UsernameOrEmail);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            var command = new LoginCommand(
                request.UsernameOrEmail,
                request.Password,
                request.RememberMe,
                ipAddress
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for {UsernameOrEmail}: {ErrorCode} - {ErrorMessage}", 
                    request.UsernameOrEmail, result.Error.Code, result.Error.Message);
                return Unauthorized(new { error = result.Error.Message, code = result.Error.Code });
            }

            _logger.LogInformation("User logged in successfully: {UserId}", result.Data!.User.Id);
            return Ok(result.Data);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/token/refresh
        ///     {
        ///        "refreshToken": "your-refresh-token-here"
        ///     }
        /// 
        /// The refresh token will be rotated (old token revoked, new token issued) for security.
        /// Access tokens expire after 1 hour. Use this endpoint to obtain a new access token.
        /// </remarks>
        /// <param name="request">Refresh token</param>
        /// <returns>New access and refresh tokens</returns>
        /// <response code="200">Token successfully refreshed</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Invalid or expired refresh token</response>
        [HttpPost("token/refresh")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            _logger.LogInformation("Token refresh attempt from IP: {IpAddress}", HttpContext.Connection.RemoteIpAddress?.ToString());
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            var command = new RefreshTokenCommand(request.RefreshToken, ipAddress);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return Unauthorized(new { error = result.Error.Message, code = result.Error.Code });

            return Ok(result.Data);
        }

        /// <summary>
        /// Logout and revoke refresh token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/logout
        ///     {
        ///        "refreshToken": "your-refresh-token-here"
        ///     }
        /// 
        /// This will revoke the refresh token, preventing further token refresh.
        /// The access token will remain valid until expiration (cannot be revoked server-side with JWT).
        /// </remarks>
        /// <param name="request">Refresh token to revoke</param>
        /// <returns>Success message</returns>
        /// <response code="204">Successfully logged out</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            _logger.LogInformation("Logout attempt from IP: {IpAddress}", HttpContext.Connection.RemoteIpAddress?.ToString());
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            var command = new LogoutCommand(request.RefreshToken, ipAddress);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/auth/password
        ///     {
        ///        "currentPassword": "OldP@ss123",
        ///        "newPassword": "NewSecureP@ss456",
        ///        "confirmPassword": "NewSecureP@ss456"
        ///     }
        /// 
        /// Requires authentication. User must provide current password for verification.
        /// New password must meet the same requirements as registration.
        /// </remarks>
        /// <param name="request">Password change details</param>
        /// <returns>Success message</returns>
        /// <response code="204">Password successfully changed</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="401">Unauthorized or invalid current password</response>
        [Authorize]
        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            _logger.LogInformation("Password change attempt for user: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User ID not found in token"));
            
            var command = new ChangePasswordCommand(
                userId,
                request.CurrentPassword,
                request.NewPassword,
                request.ConfirmPassword
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error, validationErrors = result.ValidationErrors });

            return NoContent();
        }

        /// <summary>
        /// Request password reset email
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/password/forgot
        ///     {
        ///        "email": "user@example.com"
        ///     }
        /// 
        /// Sends a password reset email if the account exists.
        /// Always returns success to prevent email enumeration attacks.
        /// Reset token expires after 60 minutes.
        /// </remarks>
        /// <param name="request">Email address</param>
        /// <returns>Generic success message</returns>
        /// <response code="202">Request accepted (check email for reset link)</response>
        /// <response code="400">Invalid input</response>
        [HttpPost("password/forgot")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);
            var command = new ForgotPasswordCommand(request.Email);
            var result = await _mediator.Send(command);

            // Always return success to prevent email enumeration
            return AcceptedAtAction(nameof(ForgotPassword), new { message = "If your email exists, you will receive a password reset link" });
        }

        /// <summary>
        /// Reset password using reset token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/password/reset
        ///     {
        ///        "email": "user@example.com",
        ///        "token": "reset-token-from-email",
        ///        "password": "NewSecureP@ss123",
        ///        "confirmPassword": "NewSecureP@ss123"
        ///     }
        /// 
        /// Reset token is sent via email and expires after 60 minutes.
        /// Password must meet the same requirements as registration.
        /// </remarks>
        /// <param name="request">Password reset details</param>
        /// <returns>Success message</returns>
        /// <response code="204">Password successfully reset</response>
        /// <response code="400">Invalid input or expired token</response>
        [HttpPost("password/reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            _logger.LogInformation("Password reset attempt for email: {Email}", request.Email);
            var command = new ResetPasswordCommand(
                request.Email,
                request.Token,
                request.Password,
                request.ConfirmPassword
            );

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error, validationErrors = result.ValidationErrors });

            return NoContent();
        }

        /// <summary>
        /// Confirm email address
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/email/confirm
        ///     {
        ///        "email": "user@example.com",
        ///        "token": "confirmation-token-from-email"
        ///     }
        /// 
        /// Confirmation token is sent via email during registration.
        /// Token expires after 24 hours.
        /// </remarks>
        /// <param name="request">Email confirmation details</param>
        /// <returns>Success message</returns>
        /// <response code="204">Email successfully confirmed</response>
        /// <response code="400">Invalid or expired token</response>
        [HttpPost("email/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            _logger.LogInformation("Email confirmation attempt for: {Email}", request.Email);
            var command = new ConfirmEmailCommand(request.Email, request.Token);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        /// <summary>
        /// Resend email confirmation
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/email/resend
        ///     {
        ///        "email": "user@example.com"
        ///     }
        /// 
        /// Sends a new confirmation email if account exists and email is not yet confirmed.
        /// Always returns success to prevent email enumeration.
        /// </remarks>
        /// <param name="request">Email address</param>
        /// <returns>Generic success message</returns>
        /// <response code="202">Request accepted</response>
        [HttpPost("email/resend")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ForgotPasswordRequest request)
        {
            var command = new ResendEmailConfirmationCommand(request.Email);
            var result = await _mediator.Send(command);

            return AcceptedAtAction(nameof(ResendEmailConfirmation), new { message = "Confirmation email sent if account exists" });
        }

        /// <summary>
        /// Enable two-factor authentication
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/2fa/enable
        ///     {
        ///        "password": "YourCurrentPassword"
        ///     }
        /// 
        /// Requires authentication and current password verification.
        /// Returns QR code URI for Google Authenticator or similar TOTP apps.
        /// Email must be confirmed before enabling 2FA.
        /// </remarks>
        /// <param name="request">Password for verification</param>
        /// <returns>2FA setup information (secret key and QR code URI)</returns>
        /// <response code="200">2FA setup information</response>
        /// <response code="400">Invalid password or email not confirmed</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpPost("2fa/enable")]
        [ProducesResponseType(typeof(TwoFactorSetupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TwoFactorSetupResponse>> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
        {
            _logger.LogInformation("2FA enable attempt for user: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User ID not found in token"));
            
            var command = new EnableTwoFactorCommand(userId, request.Password);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }

        /// <summary>
        /// Disable two-factor authentication
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/auth/2fa
        ///     {
        ///        "password": "YourCurrentPassword"
        ///     }
        /// 
        /// Requires authentication and current password verification.
        /// Removes 2FA requirement from account.
        /// </remarks>
        /// <param name="request">Password for verification</param>
        /// <returns>Success message</returns>
        /// <response code="204">2FA successfully disabled</response>
        /// <response code="400">Invalid password</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpDelete("2fa")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DisableTwoFactor([FromBody] EnableTwoFactorRequest request)
        {
            _logger.LogInformation("2FA disable attempt for user: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User ID not found in token"));
            
            var command = new DisableTwoFactorCommand(userId, request.Password);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        /// <summary>
        /// Verify two-factor authentication code
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/2fa/verify
        ///     {
        ///        "code": "123456"
        ///     }
        /// 
        /// Verifies a 6-digit TOTP code from authenticator app.
        /// Code is time-based and valid for 30 seconds.
        /// </remarks>
        /// <param name="request">6-digit verification code</param>
        /// <returns>Success message</returns>
        /// <response code="204">Code verified successfully</response>
        /// <response code="400">Invalid or expired code</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpPost("2fa/verify")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User ID not found in token"));            _logger.LogInformation("2FA disable attempt for user: {UserId}", userId);            _logger.LogInformation("2FA verification attempt for user: {UserId}", userId);
            
            var command = new VerifyTwoFactorCommand(userId, request.Code);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }
    }
}

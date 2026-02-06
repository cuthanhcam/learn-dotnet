namespace DevForge.Application.Common.Interfaces
{
    /// <summary>
    /// Provides token generation and validation for authentication
    /// </summary>
    public interface IAuthenticationTokenProvider
    {
        string GenerateAccessToken(Guid userId, string username, string email, IEnumerable<string> roles, IEnumerable<string> permissions);
        Guid? ValidateAccessToken(string token);
    }

    /// <summary>
    /// Provides notification services for application events
    /// </summary>
    public interface INotificationService
    {
        Task SendEmailConfirmationAsync(string email, string username, string confirmationLink, CancellationToken cancellationToken = default);
        Task SendPasswordResetAsync(string email, string username, string resetLink, CancellationToken cancellationToken = default);
        Task SendWelcomeEmailAsync(string email, string username, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides access to current authenticated user information
    /// </summary>
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Username { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        bool HasPermission(string permission);
    }
}

using DevForge.Domain.Entities;
using DevForge.Domain.ValueObjects;

namespace DevForge.Domain.Services
{
    /// <summary>
    /// Domain service interface for user authentication operations
    /// </summary>
    public interface IUserAuthenticationService
    {
        /// <summary>
        /// Authenticates a user with username/email and password
        /// </summary>
        Task<User> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates if a username is unique (not already taken)
        /// </summary>
        Task<bool> ValidateUniqueUsernameAsync(Username username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates if an email is unique (not already taken)
        /// </summary>
        Task<bool> ValidateUniqueEmailAsync(Email email, CancellationToken cancellationToken = default);
    }
}

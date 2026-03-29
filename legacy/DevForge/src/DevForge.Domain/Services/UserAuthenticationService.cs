using DevForge.Domain.Entities;
using DevForge.Domain.Exceptions;
using DevForge.Domain.Repositories;
using DevForge.Domain.ValueObjects;

namespace DevForge.Domain.Services
{
    /// <summary>
    /// Domain service implementation for user authentication business logic
    /// This is a domain service because it coordinates multiple entities and repositories
    /// </summary>
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserAuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
        {
            User? user = null;

            // Check if input is email or username
            if (usernameOrEmail.Contains('@'))
            {
                var email = Email.Create(usernameOrEmail);
                user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            }
            else
            {
                var username = Username.Create(usernameOrEmail);
                user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            }

            if (user == null)
                throw new DomainException("Invalid credentials");

            if (!user.CanLogin())
                throw new DomainException("User cannot login. Account may be inactive or locked.");

            // Verify password
            var passwordObj = Password.Create(password);
            if (!_passwordHasher.VerifyPassword(passwordObj, user.PasswordHash))
            {
                user.RecordFailedLoginAttempt();
                throw new DomainException("Invalid credentials");
            }

            // Record successful login
            user.RecordLogin();
            return user;
        }

        public async Task<bool> ValidateUniqueUsernameAsync(Username username, CancellationToken cancellationToken = default)
        {
            return !await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
        }

        public async Task<bool> ValidateUniqueEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return !await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        }
    }
}

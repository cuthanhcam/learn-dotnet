using DevForge.Domain.Common;
using DevForge.Domain.Events.User;
using DevForge.Domain.Exceptions;
using DevForge.Domain.ValueObjects;

namespace DevForge.Domain.Entities
{
    public class User : Entity, IAggregateRoot
    {
        public Username Username { get; private set; }
        public Email Email { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        
        // Email Confirmation
        public bool EmailConfirmed { get; private set; }
        public string? EmailConfirmationToken { get; private set; }
        public DateTime? EmailConfirmationTokenExpiresAt { get; private set; }
        
        // Phone Confirmation
        public bool PhoneNumberConfirmed { get; private set; }
        
        // Password Reset
        public string? PasswordResetToken { get; private set; }
        public DateTime? PasswordResetTokenExpiresAt { get; private set; }
        
        // Two-Factor Authentication
        public bool TwoFactorEnabled { get; private set; }
        public string? TwoFactorSecretKey { get; private set; }
        
        // Account Lockout
        public int AccessFailedCount { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public bool LockoutEnabled { get; private set; }
        
        // Audit
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        // Roles - Many to Many
        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        private User()
        {
            Username = ValueObjects.Username.Create("defaultuser");
            Email = ValueObjects.Email.Create("default@example.com");
            PasswordHash = ValueObjects.PasswordHash.Create("defaulthash00000000000000");
        }

        private User(Username username, Email email, PasswordHash passwordHash)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
            EmailConfirmed = false;
            PhoneNumberConfirmed = false;
            TwoFactorEnabled = false;
            LockoutEnabled = true;
            AccessFailedCount = 0;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public static User Create(Username username, Email email, PasswordHash passwordHash)
        {
            var user = new User(username, email, passwordHash);
            user.AddDomainEvent(new UserCreatedEvent(user.Id, username.Value, email.Value, 0));
            return user;
        }

        public void UpdateProfile(Username? username = null, Email? email = null, PhoneNumber? phoneNumber = null)
        {
            if (!IsActive)
                throw new DomainException("Cannot update inactive user profile");

            if (IsLockedOut())
                throw new DomainException("Cannot update locked user profile");

            bool hasChanges = false;

            if (username != null && username != Username)
            {
                Username = username;
                hasChanges = true;
            }

            if (email != null && email != Email)
            {
                Email = email;
                EmailConfirmed = false;
                hasChanges = true;
            }

            if (phoneNumber != null && phoneNumber != PhoneNumber)
            {
                PhoneNumber = phoneNumber;
                PhoneNumberConfirmed = false;
                hasChanges = true;
            }

            if (hasChanges)
            {
                UpdatedAt = DateTime.UtcNow;
                AddDomainEvent(new UserProfileUpdatedEvent(Id, Username.Value, Email.Value));
            }
        }

        public void ChangePassword(PasswordHash newPasswordHash)
        {
            if (!IsActive)
                throw new DomainException("Cannot change password for inactive user");

            if (IsLockedOut())
                throw new DomainException("Cannot change password for locked user");

            PasswordHash = newPasswordHash;
            PasswordResetToken = null;
            PasswordResetTokenExpiresAt = null;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserPasswordChangedEvent(Id, DateTime.UtcNow));
        }

        public void GeneratePasswordResetToken(string token, int expirationMinutes = 60)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Password reset token cannot be empty");

            PasswordResetToken = token;
            PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ValidatePasswordResetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(PasswordResetToken))
                throw new DomainException("No password reset token found");

            if (PasswordResetToken != token)
                throw new DomainException("Invalid password reset token");

            if (PasswordResetTokenExpiresAt == null || PasswordResetTokenExpiresAt < DateTime.UtcNow)
                throw new DomainException("Password reset token has expired");
        }

        public void GenerateEmailConfirmationToken(string token, int expirationHours = 24)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Email confirmation token cannot be empty");

            EmailConfirmationToken = token;
            EmailConfirmationTokenExpiresAt = DateTime.UtcNow.AddHours(expirationHours);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ConfirmEmail(string token)
        {
            if (EmailConfirmed)
                throw new DomainException("Email is already confirmed");

            if (string.IsNullOrWhiteSpace(EmailConfirmationToken))
                throw new DomainException("No email confirmation token found");

            if (EmailConfirmationToken != token)
                throw new DomainException("Invalid email confirmation token");

            if (EmailConfirmationTokenExpiresAt == null || EmailConfirmationTokenExpiresAt < DateTime.UtcNow)
                throw new DomainException("Email confirmation token has expired");

            EmailConfirmed = true;
            EmailConfirmationToken = null;
            EmailConfirmationTokenExpiresAt = null;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserEmailConfirmedEvent(Id, Email.Value));
        }

        public void ConfirmPhoneNumber()
        {
            if (PhoneNumber == null)
                throw new DomainException("No phone number to confirm");

            if (PhoneNumberConfirmed)
                throw new DomainException("Phone number is already confirmed");

            PhoneNumberConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void EnableTwoFactor(string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new DomainException("Two-factor secret key cannot be empty");

            if (!EmailConfirmed)
                throw new DomainException("Email must be confirmed before enabling two-factor authentication");

            TwoFactorEnabled = true;
            TwoFactorSecretKey = secretKey;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserTwoFactorEnabledEvent(Id));
        }

        public void DisableTwoFactor()
        {
            if (!TwoFactorEnabled)
                throw new DomainException("Two-factor authentication is not enabled");

            TwoFactorEnabled = false;
            TwoFactorSecretKey = null;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserTwoFactorDisabledEvent(Id));
        }

        public void RecordFailedLoginAttempt()
        {
            AccessFailedCount++;
            UpdatedAt = DateTime.UtcNow;

            if (LockoutEnabled && AccessFailedCount >= 5)
            {
                LockAccount(TimeSpan.FromMinutes(15));
            }

            AddDomainEvent(new UserLoginFailedEvent(Id, AccessFailedCount));
        }

        public void ResetAccessFailedCount()
        {
            if (AccessFailedCount > 0)
            {
                AccessFailedCount = 0;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void LockAccount(TimeSpan duration)
        {
            LockoutEnd = DateTime.UtcNow.Add(duration);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserLockedOutEvent(Id, LockoutEnd.Value));
        }

        public void UnlockAccount()
        {
            if (!IsLockedOut())
                throw new DomainException("Account is not locked");

            LockoutEnd = null;
            AccessFailedCount = 0;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserUnlockedEvent(Id));
        }

        public bool IsLockedOut()
        {
            return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("User is already inactive");

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserDeactivatedEvent(Id, DateTime.UtcNow));
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("User is already active");

            IsActive = true;
            LockoutEnd = null;
            AccessFailedCount = 0;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserActivatedEvent(Id, DateTime.UtcNow));
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            ResetAccessFailedCount();
            AddDomainEvent(new UserLoggedInEvent(Id, LastLoginAt.Value));
        }

        public void AssignRole(Guid roleId)
        {
            if (_userRoles.Any(ur => ur.RoleId == roleId))
                throw new DomainException("User already has this role");

            var userRole = UserRole.Create(Id, roleId);
            _userRoles.Add(userRole);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserRoleAssignedEvent(Id, roleId));
        }

        public void RemoveRole(Guid roleId)
        {
            var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null)
                throw new DomainException("User does not have this role");

            _userRoles.Remove(userRole);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new UserRoleRemovedEvent(Id, roleId));
        }

        public bool HasRole(Guid roleId)
        {
            return _userRoles.Any(ur => ur.RoleId == roleId);
        }

        public IEnumerable<Guid> GetRoleIds()
        {
            return _userRoles.Select(ur => ur.RoleId);
        }

        public bool CanLogin()
        {
            return IsActive && !IsLockedOut() && (EmailConfirmed || !LockoutEnabled);
        }
    }
}

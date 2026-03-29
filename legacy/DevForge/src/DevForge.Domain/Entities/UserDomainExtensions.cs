using DevForge.Domain.Common;
using DevForge.Domain.Events.User;
using DevForge.Domain.Exceptions;
using DevForge.Domain.Services;
using DevForge.Domain.ValueObjects;

namespace DevForge.Domain.Entities
{
    /// <summary>
    /// Extension methods for User entity to add rich domain behavior
    /// </summary>
    public static class UserDomainExtensions
    {
        /// <summary>
        /// Authenticate user with password verification
        /// </summary>
        public static void Authenticate(this User user, Password password, IPasswordHasher passwordHasher, string? ipAddress = null)
        {
            if (!user.IsActive)
                throw new DomainException("User account is not active");

            if (user.IsLockedOut())
                throw new DomainException("User account is locked out. Please try again later.");

            if (!passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                user.RecordFailedLoginAttempt();
                throw new DomainException("Invalid password");
            }

            user.RecordLogin();
        }

        /// <summary>
        /// Validates if user can perform authentication
        /// </summary>
        public static bool CanAuthenticate(this User user)
        {
            return user.IsActive && !user.IsLockedOut();
        }

        /// <summary>
        /// Validates password complexity and change password
        /// </summary>
        public static void ChangePasswordWithValidation(
            this User user, 
            Password currentPassword, 
            Password newPassword,
            IPasswordHasher passwordHasher)
        {
            if (!user.IsActive)
                throw new DomainException("Cannot change password for inactive user");

            if (user.IsLockedOut())
                throw new DomainException("Cannot change password for locked user");

            // Verify current password
            if (!passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                throw new DomainException("Current password is incorrect");

            // Ensure new password is different
            if (passwordHasher.VerifyPassword(newPassword, user.PasswordHash))
                throw new DomainException("New password must be different from current password");

            var newPasswordHash = passwordHasher.HashPassword(newPassword);
            user.ChangePassword(newPasswordHash);
        }

        /// <summary>
        /// Request password reset with validation
        /// </summary>
        public static string RequestPasswordReset(this User user, ITokenGenerator tokenGenerator)
        {
            if (!user.IsActive)
                throw new DomainException("Cannot reset password for inactive user");

            var resetToken = tokenGenerator.GeneratePasswordResetToken();
            user.GeneratePasswordResetToken(resetToken);

            return resetToken;
        }

        /// <summary>
        /// Complete password reset with token validation
        /// </summary>
        public static void CompletePasswordReset(
            this User user, 
            string token, 
            Password newPassword,
            IPasswordHasher passwordHasher)
        {
            user.ValidatePasswordResetToken(token);

            var newPasswordHash = passwordHasher.HashPassword(newPassword);
            user.ChangePassword(newPasswordHash);
        }

        /// <summary>
        /// Request email confirmation
        /// </summary>
        public static string RequestEmailConfirmation(this User user, ITokenGenerator tokenGenerator)
        {
            if (user.EmailConfirmed)
                throw new DomainException("Email is already confirmed");

            var confirmationToken = tokenGenerator.GenerateEmailConfirmationToken();
            user.GenerateEmailConfirmationToken(confirmationToken);

            return confirmationToken;
        }

        /// <summary>
        /// Setup two-factor authentication with validation
        /// </summary>
        public static (string SecretKey, string QrCodeUri) SetupTwoFactor(
            this User user,
            Password password,
            IPasswordHasher passwordHasher,
            ITwoFactorService twoFactorService)
        {
            if (!user.IsActive)
                throw new DomainException("Cannot enable 2FA for inactive user");

            if (!user.EmailConfirmed)
                throw new DomainException("Email must be confirmed before enabling 2FA");

            if (user.TwoFactorEnabled)
                throw new DomainException("Two-factor authentication is already enabled");

            // Verify password before enabling 2FA
            if (!passwordHasher.VerifyPassword(password, user.PasswordHash))
                throw new DomainException("Invalid password");

            var secretKey = twoFactorService.GenerateSecretKey();
            var qrCodeUri = twoFactorService.GenerateQrCodeUri(user.Email.Value, secretKey);

            user.EnableTwoFactor(secretKey);

            return (secretKey, qrCodeUri);
        }

        /// <summary>
        /// Verify two-factor code during authentication
        /// </summary>
        public static void VerifyTwoFactorCode(
            this User user,
            string code,
            ITwoFactorService twoFactorService)
        {
            if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecretKey))
                throw new DomainException("Two-factor authentication is not enabled");

            if (!twoFactorService.ValidateCode(user.TwoFactorSecretKey, code))
                throw new DomainException("Invalid verification code");
        }

        /// <summary>
        /// Disable two-factor with password verification
        /// </summary>
        public static void DisableTwoFactorWithValidation(
            this User user,
            Password password,
            IPasswordHasher passwordHasher)
        {
            if (!user.TwoFactorEnabled)
                throw new DomainException("Two-factor authentication is not enabled");

            // Verify password before disabling 2FA
            if (!passwordHasher.VerifyPassword(password, user.PasswordHash))
                throw new DomainException("Invalid password");

            user.DisableTwoFactor();
        }

        /// <summary>
        /// Check if user has specific permission through roles
        /// </summary>
        public static bool HasPermission(this User user, string permissionName, IEnumerable<Role> roles)
        {
            var userRoleIds = user.GetRoleIds();
            var userRoles = roles.Where(r => userRoleIds.Contains(r.Id));

            foreach (var role in userRoles)
            {
                var rolePermissionIds = role.RolePermissions.Select(rp => rp.PermissionId);
                // Would need to check against actual permissions
                // This is a simplified version
            }

            return false; // Placeholder - needs full implementation with permissions
        }

        /// <summary>
        /// Validate user state before critical operations
        /// </summary>
        public static void EnsureCanPerformOperation(this User user, string operationName)
        {
            if (!user.IsActive)
                throw new DomainException($"Cannot perform {operationName}: User account is not active");

            if (user.IsLockedOut())
                throw new DomainException($"Cannot perform {operationName}: User account is locked");
        }

        /// <summary>
        /// Check if user needs to change password (password age policy)
        /// </summary>
        public static bool ShouldChangePassword(this User user, int maxPasswordAgeDays = 90)
        {
            if (user.UpdatedAt == null)
                return false;

            var daysSincePasswordChange = (DateTime.UtcNow - user.UpdatedAt.Value).Days;
            return daysSincePasswordChange >= maxPasswordAgeDays;
        }

        /// <summary>
        /// Get user security score (0-100)
        /// </summary>
        public static int GetSecurityScore(this User user)
        {
            int score = 0;

            if (user.EmailConfirmed) score += 25;
            if (user.PhoneNumberConfirmed) score += 15;
            if (user.TwoFactorEnabled) score += 30;
            if (!user.IsLockedOut()) score += 15;
            if (user.AccessFailedCount == 0) score += 15;

            return Math.Min(100, score);
        }

        /// <summary>
        /// Check if account needs attention (security warnings)
        /// </summary>
        public static List<string> GetSecurityWarnings(this User user)
        {
            var warnings = new List<string>();

            if (!user.EmailConfirmed)
                warnings.Add("Email not confirmed");

            if (!user.TwoFactorEnabled)
                warnings.Add("Two-factor authentication not enabled");

            if (user.AccessFailedCount > 0)
                warnings.Add($"{user.AccessFailedCount} failed login attempts");

            if (user.ShouldChangePassword())
                warnings.Add("Password should be changed");

            if (!user.IsActive)
                warnings.Add("Account is inactive");

            return warnings;
        }
    }
}

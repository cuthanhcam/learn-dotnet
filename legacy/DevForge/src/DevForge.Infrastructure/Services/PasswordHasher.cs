using DevForge.Domain.Services;
using DevForge.Domain.ValueObjects;

namespace DevForge.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public PasswordHash HashPassword(Password password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password.Value, BCrypt.Net.BCrypt.GenerateSalt(12));
            return PasswordHash.Create(hash);
        }

        public bool VerifyPassword(Password password, PasswordHash passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password.Value, passwordHash.Value);
            }
            catch
            {
                return false;
            }
        }
    }
}

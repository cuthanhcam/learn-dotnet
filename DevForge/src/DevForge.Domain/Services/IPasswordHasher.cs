using DevForge.Domain.ValueObjects;

namespace DevForge.Domain.Services
{
    public interface IPasswordHasher
    {
        PasswordHash HashPassword(Password password);
        bool VerifyPassword(Password password, PasswordHash passwordHash);
    }
}

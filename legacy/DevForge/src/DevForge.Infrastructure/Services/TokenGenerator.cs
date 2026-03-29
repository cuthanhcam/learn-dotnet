using DevForge.Domain.Services;
using System.Security.Cryptography;

namespace DevForge.Infrastructure.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateEmailConfirmationToken()
        {
            return GenerateSecureToken(32);
        }

        public string GeneratePasswordResetToken()
        {
            return GenerateSecureToken(32);
        }

        public string GenerateTwoFactorCode()
        {
            var randomNumber = new byte[4];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var code = BitConverter.ToInt32(randomNumber, 0) % 1000000;
            return Math.Abs(code).ToString("D6");
        }

        private string GenerateSecureToken(int length)
        {
            var randomNumber = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, length);
        }
    }
}

using DevForge.Domain.Services;
using OtpNet;
using System.Text;

namespace DevForge.Infrastructure.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        public string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUri(string email, string secretKey)
        {
            var issuer = "DevForge";
            return $"otpauth://totp/{issuer}:{email}?secret={secretKey}&issuer={issuer}";
        }

        public bool ValidateCode(string secretKey, string code)
        {
            try
            {
                var key = Base32Encoding.ToBytes(secretKey);
                var totp = new Totp(key);
                
                return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
            }
            catch
            {
                return false;
            }
        }

        public List<string> GenerateBackupCodes(int count = 10)
        {
            var codes = new List<string>();
            for (int i = 0; i < count; i++)
            {
                codes.Add(Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());
            }
            return codes;
        }
    }
}

namespace DevForge.Domain.Services
{
    /// <summary>
    /// Service for generating and validating two-factor authentication codes
    /// </summary>
    public interface ITwoFactorService
    {
        /// <summary>
        /// Generates a new secret key for TOTP
        /// </summary>
        string GenerateSecretKey();

        /// <summary>
        /// Generates QR code URI for authenticator apps
        /// </summary>
        string GenerateQrCodeUri(string email, string secretKey);

        /// <summary>
        /// Validates a TOTP code against the secret key
        /// </summary>
        bool ValidateCode(string secretKey, string code);

        /// <summary>
        /// Generates backup/recovery codes
        /// </summary>
        List<string> GenerateBackupCodes(int count = 10);
    }
}

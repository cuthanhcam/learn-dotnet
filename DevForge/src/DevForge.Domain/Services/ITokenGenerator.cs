namespace DevForge.Domain.Services
{
    public interface ITokenGenerator
    {
        string GenerateRefreshToken();
        string GenerateEmailConfirmationToken();
        string GeneratePasswordResetToken();
        string GenerateTwoFactorCode();
    }
}

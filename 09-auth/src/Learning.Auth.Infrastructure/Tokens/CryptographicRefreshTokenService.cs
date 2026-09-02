using System.Security.Cryptography;
using System.Text;
using Learning.Auth.Application.Abstractions;
using Microsoft.AspNetCore.WebUtilities;

namespace Learning.Auth.Infrastructure.Tokens;

public sealed class CryptographicRefreshTokenService(JwtOptions options) : IRefreshTokenService
{
    public IssuedRefreshToken Issue(DateTimeOffset now)
    {
        options.Validate();
        // 256 random bits make online guessing and offline token enumeration impractical.
        string value = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        return new IssuedRefreshToken(value, ComputeDigest(value), now.AddDays(options.RefreshTokenDays));
    }

    public string ComputeDigest(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}

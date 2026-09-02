using Learning.Auth.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Learning.Auth.Infrastructure.Identity;

/// <summary>
/// Adapts the supported ASP.NET Core Identity password hasher instead of implementing a custom
/// PBKDF2 format. Its encoded output carries algorithm parameters and a random salt.
/// </summary>
public sealed class AspNetCorePasswordHashService : IPasswordHashService
{
    private static readonly object HashSubject = new();
    private readonly PasswordHasher<object> _hasher = new();
    private readonly string _unknownAccountHash;

    public AspNetCorePasswordHashService() =>
        _unknownAccountHash = _hasher.HashPassword(HashSubject, "not-a-real-account-password");

    public string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return _hasher.HashPassword(HashSubject, password);
    }

    public PasswordVerification Verify(string passwordHash, string suppliedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentNullException.ThrowIfNull(suppliedPassword);

        PasswordVerificationResult result =
            _hasher.VerifyHashedPassword(HashSubject, passwordHash, suppliedPassword);

        return result switch
        {
            PasswordVerificationResult.Success => PasswordVerification.Succeeded,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordVerification.SucceededRehashNeeded,
            _ => PasswordVerification.Failed
        };
    }

    public void VerifyUnknownAccount(string suppliedPassword)
    {
        ArgumentNullException.ThrowIfNull(suppliedPassword);
        _ = _hasher.VerifyHashedPassword(HashSubject, _unknownAccountHash, suppliedPassword);
    }
}

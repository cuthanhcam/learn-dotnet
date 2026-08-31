namespace Learning.Auth.Application.Abstractions;

public sealed record IssuedRefreshToken(string Value, string Digest, DateTimeOffset ExpiresAt);

public interface IRefreshTokenService
{
    IssuedRefreshToken Issue(DateTimeOffset now);
    string ComputeDigest(string token);
}

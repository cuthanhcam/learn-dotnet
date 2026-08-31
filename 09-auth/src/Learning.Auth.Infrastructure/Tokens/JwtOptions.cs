namespace Learning.Auth.Infrastructure.Tokens;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public int AccessTokenMinutes { get; init; } = 10;

    public void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(Audience);
        ArgumentException.ThrowIfNullOrWhiteSpace(SigningKey);

        if (System.Text.Encoding.UTF8.GetByteCount(SigningKey) < 32)
            throw new InvalidOperationException("Jwt:SigningKey must contain at least 256 bits of key material.");
        if (AccessTokenMinutes is < 1 or > 60)
            throw new InvalidOperationException("Jwt:AccessTokenMinutes must be between 1 and 60.");
    }
}

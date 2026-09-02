using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;
using Microsoft.IdentityModel.Tokens;

namespace Learning.Auth.Infrastructure.Tokens;

public sealed class JwtAccessTokenIssuer(JwtOptions options, TimeProvider timeProvider) : IAccessTokenIssuer
{
    public AccessToken Issue(UserAccount account)
    {
        ArgumentNullException.ThrowIfNull(account);
        options.Validate();

        DateTimeOffset issuedAt = timeProvider.GetUtcNow();
        DateTimeOffset expiresAt = issuedAt.AddMinutes(options.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, account.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };
        claims.AddRange(account.Roles.Select(role => new Claim(JwtClaimNames.Role, role)));
        claims.AddRange(account.Permissions.Select(permission => new Claim(JwtClaimNames.Scope, permission)));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt.UtcDateTime,
            NotBefore = issuedAt.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        return new AccessToken(handler.WriteToken(handler.CreateToken(descriptor)), expiresAt);
    }
}

public static class JwtClaimNames
{
    public const string Role = "role";
    public const string Scope = "scope";
}

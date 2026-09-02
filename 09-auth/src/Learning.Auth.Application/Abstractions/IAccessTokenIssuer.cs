using Learning.Auth.Domain.Users;

namespace Learning.Auth.Application.Abstractions;

public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

public interface IAccessTokenIssuer
{
    AccessToken Issue(UserAccount account);
}

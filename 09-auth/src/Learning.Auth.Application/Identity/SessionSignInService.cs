using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Sessions;

namespace Learning.Auth.Application.Identity;

public sealed record SessionTokens(AccessToken AccessToken, string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);

public sealed record SessionSignInResult(SignInStatus Status, SessionTokens? Tokens = null);

public sealed class SessionSignInService(CredentialSignInService credentials,
    IAccessTokenIssuer accessTokens, IRefreshTokenService refreshTokens,
    IRefreshSessionStore sessions, TimeProvider timeProvider)
{
    public async Task<SessionSignInResult> SignInAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        SignInResult result = await credentials.VerifyAsync(email, password, cancellationToken)
            .ConfigureAwait(false);
        if (result is not { Status: SignInStatus.Succeeded, Account: not null })
            return new SessionSignInResult(result.Status);

        DateTimeOffset now = timeProvider.GetUtcNow();
        IssuedRefreshToken refreshToken = refreshTokens.Issue(now);
        RefreshSession session = RefreshSession.Issue(Guid.NewGuid(), Guid.NewGuid(), result.Account.Id,
            refreshToken.Digest, now, refreshToken.ExpiresAt);
        await sessions.AddAsync(session, cancellationToken).ConfigureAwait(false);

        return new SessionSignInResult(result.Status, new SessionTokens(
            accessTokens.Issue(result.Account), refreshToken.Value, refreshToken.ExpiresAt));
    }
}

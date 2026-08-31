using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;

namespace Learning.Auth.Application.Identity;

public sealed record RefreshResult(RefreshRotationStatus Status, SessionTokens? Tokens = null);

public sealed class RefreshSessionService(IRefreshSessionStore sessions, IRefreshTokenService refreshTokens,
    IUserAccountRepository accounts, IAccessTokenIssuer accessTokens, TimeProvider timeProvider)
{
    public async Task<RefreshResult> RefreshAsync(string presentedToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(presentedToken) || presentedToken.Length > 512)
            return new RefreshResult(RefreshRotationStatus.NotFound);

        DateTimeOffset now = timeProvider.GetUtcNow();
        string presentedDigest = refreshTokens.ComputeDigest(presentedToken);
        IssuedRefreshToken replacement = refreshTokens.Issue(now);
        RefreshRotationResult rotation = await sessions.RotateAsync(presentedDigest,
            replacement.Digest, replacement.ExpiresAt, now, cancellationToken).ConfigureAwait(false);

        if (rotation is not { Status: RefreshRotationStatus.Rotated, UserId: not null })
            return new RefreshResult(rotation.Status);

        UserAccount? account = await accounts.FindByIdAsync(rotation.UserId.Value, cancellationToken)
            .ConfigureAwait(false);
        if (account is null || account.Status != AccountStatus.Active)
        {
            await sessions.RevokeFamilyAsync(replacement.Digest, now, cancellationToken).ConfigureAwait(false);
            return new RefreshResult(RefreshRotationStatus.Revoked);
        }

        return new RefreshResult(rotation.Status, new SessionTokens(
            accessTokens.Issue(account), replacement.Value, replacement.ExpiresAt));
    }

    public Task RevokeAsync(string presentedToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(presentedToken) || presentedToken.Length > 512)
            return Task.CompletedTask;
        return sessions.RevokeFamilyAsync(refreshTokens.ComputeDigest(presentedToken),
            timeProvider.GetUtcNow(), cancellationToken);
    }
}

using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;

namespace Learning.Auth.Application.Identity;

public sealed record RefreshResult(RefreshRotationStatus Status, SessionTokens? Tokens = null);

public sealed class RefreshSessionService(IRefreshSessionStore sessions, IRefreshTokenService refreshTokens,
    IUserAccountRepository accounts, IAccessTokenIssuer accessTokens, TimeProvider timeProvider,
    ISecurityEventSink securityEvents)
{
    public async Task<RefreshResult> RefreshAsync(string presentedToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(presentedToken) || presentedToken.Length > 512)
        {
            await WriteEventAsync(SecurityEventType.RefreshRejected, null, cancellationToken).ConfigureAwait(false);
            return new RefreshResult(RefreshRotationStatus.NotFound);
        }

        DateTimeOffset now = timeProvider.GetUtcNow();
        string presentedDigest = refreshTokens.ComputeDigest(presentedToken);
        IssuedRefreshToken replacement = refreshTokens.Issue(now);
        RefreshRotationResult rotation = await sessions.RotateAsync(presentedDigest,
            replacement.Digest, replacement.ExpiresAt, now, cancellationToken).ConfigureAwait(false);

        if (rotation is not { Status: RefreshRotationStatus.Rotated, UserId: not null })
        {
            SecurityEventType eventType = rotation.Status == RefreshRotationStatus.ReplayDetected
                ? SecurityEventType.RefreshReplayDetected
                : SecurityEventType.RefreshRejected;
            await WriteEventAsync(eventType, rotation.UserId, cancellationToken).ConfigureAwait(false);
            return new RefreshResult(rotation.Status);
        }

        UserAccount? account = await accounts.FindByIdAsync(rotation.UserId.Value, cancellationToken)
            .ConfigureAwait(false);
        if (account is null || account.Status != AccountStatus.Active)
        {
            await sessions.RevokeFamilyAsync(replacement.Digest, now, cancellationToken).ConfigureAwait(false);
            await WriteEventAsync(SecurityEventType.RefreshRejected, rotation.UserId, cancellationToken)
                .ConfigureAwait(false);
            return new RefreshResult(RefreshRotationStatus.Revoked);
        }

        await WriteEventAsync(SecurityEventType.RefreshRotated, account.Id, cancellationToken).ConfigureAwait(false);
        return new RefreshResult(rotation.Status, new SessionTokens(
            accessTokens.Issue(account), replacement.Value, replacement.ExpiresAt));
    }

    public async Task RevokeAsync(string presentedToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(presentedToken) || presentedToken.Length > 512)
            return;
        await sessions.RevokeFamilyAsync(refreshTokens.ComputeDigest(presentedToken),
            timeProvider.GetUtcNow(), cancellationToken).ConfigureAwait(false);
        await WriteEventAsync(SecurityEventType.SessionRevoked, null, cancellationToken).ConfigureAwait(false);
    }

    private ValueTask WriteEventAsync(SecurityEventType type, Guid? subjectId,
        CancellationToken cancellationToken) => securityEvents.WriteAsync(
            new SecurityEvent(type, timeProvider.GetUtcNow(), subjectId), cancellationToken);
}

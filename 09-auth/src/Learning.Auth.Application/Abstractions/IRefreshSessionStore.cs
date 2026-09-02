using Learning.Auth.Domain.Sessions;

namespace Learning.Auth.Application.Abstractions;

public enum RefreshRotationStatus { Rotated, NotFound, Expired, Revoked, ReplayDetected }

public sealed record RefreshRotationResult(RefreshRotationStatus Status, Guid? UserId = null);

public interface IRefreshSessionStore
{
    Task AddAsync(RefreshSession session, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically consumes one session and inserts its replacement. Reuse revokes the whole family.
    /// </summary>
    Task<RefreshRotationResult> RotateAsync(string presentedDigest, string replacementDigest,
        DateTimeOffset replacementExpiresAt, DateTimeOffset now, CancellationToken cancellationToken);

    Task RevokeFamilyAsync(string presentedDigest, DateTimeOffset now, CancellationToken cancellationToken);
}

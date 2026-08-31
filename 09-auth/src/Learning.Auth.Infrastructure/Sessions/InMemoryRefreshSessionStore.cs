using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Sessions;

namespace Learning.Auth.Infrastructure.Sessions;

/// <summary>
/// Serializes state transitions with one lock to demonstrate the atomic database transaction that
/// a production adapter must provide. A concurrent replay can never create two valid replacements.
/// </summary>
public sealed class InMemoryRefreshSessionStore : IRefreshSessionStore
{
    private readonly Lock _gate = new();
    private readonly Dictionary<string, RefreshSession> _byDigest = new(StringComparer.Ordinal);

    public Task AddAsync(RefreshSession session, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            if (!_byDigest.TryAdd(session.TokenDigest, session))
                throw new InvalidOperationException("A refresh-token digest collision was detected.");
        }
        return Task.CompletedTask;
    }

    public Task<RefreshRotationResult> RotateAsync(string presentedDigest, string replacementDigest,
        DateTimeOffset replacementExpiresAt, DateTimeOffset now, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            if (!_byDigest.TryGetValue(presentedDigest, out RefreshSession? current))
                return Task.FromResult(new RefreshRotationResult(RefreshRotationStatus.NotFound));

            if (current.UsedAt is not null)
            {
                RevokeFamily(current.FamilyId, now);
                return Task.FromResult(new RefreshRotationResult(RefreshRotationStatus.ReplayDetected));
            }
            if (current.RevokedAt is not null)
                return Task.FromResult(new RefreshRotationResult(RefreshRotationStatus.Revoked));
            if (current.IsExpired(now))
            {
                current.Revoke(now);
                return Task.FromResult(new RefreshRotationResult(RefreshRotationStatus.Expired));
            }
            if (_byDigest.ContainsKey(replacementDigest))
                throw new InvalidOperationException("A refresh-token digest collision was detected.");

            RefreshSession replacement = RefreshSession.Issue(Guid.NewGuid(), current.FamilyId,
                current.UserId, replacementDigest, now, replacementExpiresAt);
            current.MarkRotated(replacement.Id, now);
            _byDigest.Add(replacement.TokenDigest, replacement);
            return Task.FromResult(new RefreshRotationResult(RefreshRotationStatus.Rotated, current.UserId));
        }
    }

    public Task RevokeFamilyAsync(string presentedDigest, DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            if (_byDigest.TryGetValue(presentedDigest, out RefreshSession? session))
                RevokeFamily(session.FamilyId, now);
        }
        return Task.CompletedTask;
    }

    private void RevokeFamily(Guid familyId, DateTimeOffset now)
    {
        foreach (RefreshSession session in _byDigest.Values.Where(value => value.FamilyId == familyId))
            session.Revoke(now);
    }
}

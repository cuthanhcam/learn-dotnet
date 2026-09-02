namespace Learning.Auth.Domain.Sessions;

/// <summary>
/// A server-side refresh session. Raw bearer credentials never enter this entity; only a digest is stored.
/// </summary>
public sealed class RefreshSession
{
    private RefreshSession(Guid id, Guid familyId, Guid userId, string tokenDigest,
        DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        Id = id;
        FamilyId = familyId;
        UserId = userId;
        TokenDigest = tokenDigest;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; }
    public Guid FamilyId { get; }
    public Guid UserId { get; }
    public string TokenDigest { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
    public DateTimeOffset? UsedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedBySessionId { get; private set; }

    public static RefreshSession Issue(Guid id, Guid familyId, Guid userId, string tokenDigest,
        DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        if (id == Guid.Empty || familyId == Guid.Empty || userId == Guid.Empty)
            throw new ArgumentException("Session, family, and user identifiers are required.");
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenDigest);
        if (expiresAt <= createdAt)
            throw new ArgumentOutOfRangeException(nameof(expiresAt), "Expiration must follow issuance.");
        return new RefreshSession(id, familyId, userId, tokenDigest, createdAt, expiresAt);
    }

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    public void MarkRotated(Guid replacementId, DateTimeOffset now)
    {
        if (replacementId == Guid.Empty)
            throw new ArgumentException("A replacement identifier is required.", nameof(replacementId));
        if (UsedAt is not null || RevokedAt is not null)
            throw new InvalidOperationException("Only an active refresh session can rotate.");
        UsedAt = now;
        ReplacedBySessionId = replacementId;
    }

    public void Revoke(DateTimeOffset now) => RevokedAt ??= now;
}

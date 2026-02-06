namespace DevForge.Domain.Events.RefreshToken
{
    public class RefreshTokenRevokedEvent : DomainEvent
    {
        public Guid TokenId { get; }
        public Guid UserId { get; }
        public DateTime RevokedAt { get; }
        public string RevokedByIp { get; }
        public string Reason { get; }

        public RefreshTokenRevokedEvent(Guid tokenId, Guid userId, DateTime revokedAt, string revokedByIp, string reason)
        {
            TokenId = tokenId;
            UserId = userId;
            RevokedAt = revokedAt;
            RevokedByIp = revokedByIp;
            Reason = reason;
        }
    }
}

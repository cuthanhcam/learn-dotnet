namespace DevForge.Domain.Events.RefreshToken
{
    public class RefreshTokenCreatedEvent : DomainEvent
    {
        public Guid TokenId { get; }
        public Guid UserId { get; }
        public DateTime ExpiresAt { get; }

        public RefreshTokenCreatedEvent(Guid tokenId, Guid userId, DateTime expiresAt)
        {
            TokenId = tokenId;
            UserId = userId;
            ExpiresAt = expiresAt;
        }
    }
}

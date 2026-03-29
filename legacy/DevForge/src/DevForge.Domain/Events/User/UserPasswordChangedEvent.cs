namespace DevForge.Domain.Events.User
{
    public class UserPasswordChangedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public DateTime ChangedAt { get; }

        public UserPasswordChangedEvent(Guid userId, DateTime changedAt)
        {
            UserId = userId;
            ChangedAt = changedAt;
        }
    }
}

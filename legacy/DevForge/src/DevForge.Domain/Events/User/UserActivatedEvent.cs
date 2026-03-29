namespace DevForge.Domain.Events.User
{
    public class UserActivatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public DateTime ActivatedAt { get; }

        public UserActivatedEvent(Guid userId, DateTime activatedAt)
        {
            UserId = userId;
            ActivatedAt = activatedAt;
        }
    }
}

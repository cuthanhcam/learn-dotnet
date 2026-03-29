namespace DevForge.Domain.Events.User
{
    public class UserDeactivatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public DateTime DeactivatedAt { get; }

        public UserDeactivatedEvent(Guid userId, DateTime deactivatedAt)
        {
            UserId = userId;
            DeactivatedAt = deactivatedAt;
        }
    }
}

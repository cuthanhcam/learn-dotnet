using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserUnlockedEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserUnlockedEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}

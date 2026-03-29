using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserLockedOutEvent : DomainEvent
    {
        public Guid UserId { get; }
        public DateTime LockoutEnd { get; }

        public UserLockedOutEvent(Guid userId, DateTime lockoutEnd)
        {
            UserId = userId;
            LockoutEnd = lockoutEnd;
        }
    }
}

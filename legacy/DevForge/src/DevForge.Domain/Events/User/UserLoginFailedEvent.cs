using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserLoginFailedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public int FailedAttempts { get; }

        public UserLoginFailedEvent(Guid userId, int failedAttempts)
        {
            UserId = userId;
            FailedAttempts = failedAttempts;
        }
    }
}

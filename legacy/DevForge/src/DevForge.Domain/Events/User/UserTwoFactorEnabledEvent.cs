using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserTwoFactorEnabledEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserTwoFactorEnabledEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}

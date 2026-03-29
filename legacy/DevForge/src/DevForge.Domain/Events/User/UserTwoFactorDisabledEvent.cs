using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserTwoFactorDisabledEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserTwoFactorDisabledEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}

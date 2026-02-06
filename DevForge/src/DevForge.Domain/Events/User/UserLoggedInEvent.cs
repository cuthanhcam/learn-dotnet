using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserLoggedInEvent : DomainEvent
    {
        public Guid UserId { get; }
        public DateTime LoginTime { get; }

        public UserLoggedInEvent(Guid userId, DateTime loginTime)
        {
            UserId = userId;
            LoginTime = loginTime;
        }
    }
}

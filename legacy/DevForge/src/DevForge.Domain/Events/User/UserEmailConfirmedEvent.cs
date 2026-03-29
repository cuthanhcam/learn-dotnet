using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserEmailConfirmedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }

        public UserEmailConfirmedEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}

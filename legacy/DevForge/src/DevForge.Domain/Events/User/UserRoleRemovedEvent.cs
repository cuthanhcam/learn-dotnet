using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserRoleRemovedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public Guid RoleId { get; }

        public UserRoleRemovedEvent(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}

using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    public class UserRoleAssignedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public Guid RoleId { get; }

        public UserRoleAssignedEvent(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}

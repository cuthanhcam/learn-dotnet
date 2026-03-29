using DevForge.Domain.Events;

namespace DevForge.Domain.Events.Role
{
    public class RoleCreatedEvent : DomainEvent
    {
        public Guid RoleId { get; }
        public string Name { get; }
        public bool IsSystemRole { get; }

        public RoleCreatedEvent(Guid roleId, string name, bool isSystemRole)
        {
            RoleId = roleId;
            Name = name;
            IsSystemRole = isSystemRole;
        }
    }
}

using DevForge.Domain.Events;

namespace DevForge.Domain.Events.Role
{
    public class RoleUpdatedEvent : DomainEvent
    {
        public Guid RoleId { get; }
        public string Name { get; }
        public string Description { get; }

        public RoleUpdatedEvent(Guid roleId, string name, string description)
        {
            RoleId = roleId;
            Name = name;
            Description = description;
        }
    }
}

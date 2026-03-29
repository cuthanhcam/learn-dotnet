using DevForge.Domain.Events;

namespace DevForge.Domain.Events.Permission
{
    public class PermissionCreatedEvent : DomainEvent
    {
        public Guid PermissionId { get; }
        public string Name { get; }
        public string Category { get; }

        public PermissionCreatedEvent(Guid permissionId, string name, string category)
        {
            PermissionId = permissionId;
            Name = name;
            Category = category;
        }
    }
}

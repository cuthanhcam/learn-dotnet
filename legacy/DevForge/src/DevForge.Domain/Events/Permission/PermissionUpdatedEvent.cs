using DevForge.Domain.Events;

namespace DevForge.Domain.Events.Permission
{
    public class PermissionUpdatedEvent : DomainEvent
    {
        public Guid PermissionId { get; }
        public string Description { get; }
        public string Category { get; }

        public PermissionUpdatedEvent(Guid permissionId, string description, string category)
        {
            PermissionId = permissionId;
            Description = description;
            Category = category;
        }
    }
}

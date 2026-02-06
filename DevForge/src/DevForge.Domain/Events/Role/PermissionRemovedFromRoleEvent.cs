using DevForge.Domain.Events;

namespace DevForge.Domain.Events.Role
{
    public class PermissionRemovedFromRoleEvent : DomainEvent
    {
        public Guid RoleId { get; }
        public Guid PermissionId { get; }

        public PermissionRemovedFromRoleEvent(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}

using DevForge.Domain.Events;

namespace DevForge.Domain.Events.Role
{
    public class PermissionAddedToRoleEvent : DomainEvent
    {
        public Guid RoleId { get; }
        public Guid PermissionId { get; }
        public string PermissionName { get; }

        public PermissionAddedToRoleEvent(Guid roleId, Guid permissionId, string permissionName)
        {
            RoleId = roleId;
            PermissionId = permissionId;
            PermissionName = permissionName;
        }
    }
}

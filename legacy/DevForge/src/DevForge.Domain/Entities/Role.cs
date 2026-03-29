using DevForge.Domain.Common;
using DevForge.Domain.Events.Role;
using DevForge.Domain.Exceptions;

namespace DevForge.Domain.Entities
{
    public class Role : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsSystemRole { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Role()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        private Role(string name, string description, bool isSystemRole = false)
        {
            Name = name;
            Description = description;
            IsSystemRole = isSystemRole;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public static Role Create(string name, string description, bool isSystemRole = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name cannot be empty");

            if (name.Length > 50)
                throw new DomainException("Role name cannot exceed 50 characters");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Role description cannot be empty");

            var role = new Role(name, description, isSystemRole);
            role.AddDomainEvent(new RoleCreatedEvent(role.Id, name, isSystemRole));
            return role;
        }

        public void UpdateDetails(string name, string description)
        {
            if (IsSystemRole)
                throw new DomainException("Cannot modify system roles");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name cannot be empty");

            if (name.Length > 50)
                throw new DomainException("Role name cannot exceed 50 characters");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Role description cannot be empty");

            Name = name;
            Description = description;
            AddDomainEvent(new RoleUpdatedEvent(Id, name, description));
        }

        public void AddPermission(Permission permission)
        {
            if (permission == null)
                throw new DomainException("Permission cannot be null");

            if (_rolePermissions.Any(rp => rp.PermissionId == permission.Id))
                throw new DomainException($"Permission '{permission.Name}' is already assigned to this role");

            var rolePermission = RolePermission.Create(Id, permission.Id);
            _rolePermissions.Add(rolePermission);
            AddDomainEvent(new PermissionAddedToRoleEvent(Id, permission.Id, permission.Name));
        }

        public void RemovePermission(Guid permissionId)
        {
            if (IsSystemRole)
                throw new DomainException("Cannot modify permissions of system roles");

            var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (rolePermission == null)
                throw new DomainException("Permission not found in this role");

            _rolePermissions.Remove(rolePermission);
            AddDomainEvent(new PermissionRemovedFromRoleEvent(Id, permissionId));
        }

        public bool HasPermission(Guid permissionId)
        {
            return _rolePermissions.Any(rp => rp.PermissionId == permissionId);
        }

        public IEnumerable<Guid> GetPermissionIds()
        {
            return _rolePermissions.Select(rp => rp.PermissionId);
        }
    }
}

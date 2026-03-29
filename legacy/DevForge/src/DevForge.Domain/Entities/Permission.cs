using DevForge.Domain.Common;
using DevForge.Domain.Events.Permission;
using DevForge.Domain.Exceptions;

namespace DevForge.Domain.Entities
{
    public class Permission : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Permission()
        {
            Name = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
        }

        private Permission(string name, string description, string category)
        {
            Name = name;
            Description = description;
            Category = category;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public static Permission Create(string name, string description, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Permission name cannot be empty");

            if (name.Length > 100)
                throw new DomainException("Permission name cannot exceed 100 characters");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Permission description cannot be empty");

            if (string.IsNullOrWhiteSpace(category))
                throw new DomainException("Permission category cannot be empty");

            var permission = new Permission(name, description, category);
            permission.AddDomainEvent(new PermissionCreatedEvent(permission.Id, name, category));
            return permission;
        }

        public void UpdateDetails(string description, string category)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Permission description cannot be empty");

            if (string.IsNullOrWhiteSpace(category))
                throw new DomainException("Permission category cannot be empty");

            Description = description;
            Category = category;
            AddDomainEvent(new PermissionUpdatedEvent(Id, description, category));
        }
    }
}

using DevForge.Domain.Common;
using DevForge.Domain.Exceptions;

namespace DevForge.Domain.Entities
{
    public class RolePermission : Entity
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }
        public DateTime AssignedAt { get; private set; }

        private RolePermission()
        {
        }

        private RolePermission(Guid roleId, Guid permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
            AssignedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public static RolePermission Create(Guid roleId, Guid permissionId)
        {
            if (roleId == Guid.Empty)
                throw new DomainException("RoleId cannot be empty");

            if (permissionId == Guid.Empty)
                throw new DomainException("PermissionId cannot be empty");

            return new RolePermission(roleId, permissionId);
        }
    }
}

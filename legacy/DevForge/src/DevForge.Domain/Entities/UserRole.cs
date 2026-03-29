using DevForge.Domain.Common;
using DevForge.Domain.Exceptions;

namespace DevForge.Domain.Entities
{
    public class UserRole : Entity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public DateTime AssignedAt { get; private set; }

        private UserRole()
        {
        }

        private UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
            AssignedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public static UserRole Create(Guid userId, Guid roleId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (roleId == Guid.Empty)
                throw new DomainException("RoleId cannot be empty");

            return new UserRole(userId, roleId);
        }
    }
}

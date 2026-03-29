using DevForge.Domain.Entities;
using DevForge.Domain.Exceptions;
using DevForge.Domain.Repositories;

namespace DevForge.Domain.Services
{
    public class RolePermissionService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RolePermissionService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _permissionRepository.GetPermissionsByUserIdAsync(userId, cancellationToken);
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default)
        {
            var permissions = await _permissionRepository.GetPermissionsByUserIdAsync(userId, cancellationToken);
            return permissions.Any(p => p.Name == permissionName);
        }

        public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                throw new DomainException("Role not found");

            var permission = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (permission == null)
                throw new DomainException("Permission not found");

            role.AddPermission(permission);
            await _roleRepository.UpdateAsync(role, cancellationToken);
        }

        public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                throw new DomainException("Role not found");

            role.RemovePermission(permissionId);
            await _roleRepository.UpdateAsync(role, cancellationToken);
        }
    }
}

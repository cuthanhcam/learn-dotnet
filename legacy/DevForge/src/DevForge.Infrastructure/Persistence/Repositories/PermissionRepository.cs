using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using DevForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevForge.Infrastructure.Persistence.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Join(_dbSet,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            return await _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Join(_dbSet,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}

using DevForge.Domain.Common;
using DevForge.Domain.Entities;

namespace DevForge.Domain.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Role>> GetRolesWithPermissionsAsync(CancellationToken cancellationToken = default);
        Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

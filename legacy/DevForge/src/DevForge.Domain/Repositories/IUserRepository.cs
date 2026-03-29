using DevForge.Domain.Common;
using DevForge.Domain.Entities;
using DevForge.Domain.ValueObjects;

namespace DevForge.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(Username username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailConfirmationTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    }
}

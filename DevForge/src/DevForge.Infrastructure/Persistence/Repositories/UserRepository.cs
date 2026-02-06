using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using DevForge.Domain.Specifications.Users;
using DevForge.Domain.ValueObjects;
using DevForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevForge.Infrastructure.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Username.Value == username.Value, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(Username username, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(u => u.Username.Value == username.Value, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
        }

        public async Task<User?> GetByEmailConfirmationTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token, cancellationToken);
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            // Using Specification Pattern
            var specification = new UserByRoleSpecification(roleId);
            
            return await _dbSet
                .Include(u => u.UserRoles)
                .Where(specification.ToExpression())
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            // Using Specification Pattern
            var specification = new ActiveUserSpecification();
            
            return await _dbSet
                .Include(u => u.UserRoles)
                .Where(specification.ToExpression())
                .ToListAsync(cancellationToken);
        }

        public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
    }
}

using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using DevForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevForge.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > now)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetAllTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string revokedByIp, CancellationToken cancellationToken = default)
        {
            var activeTokens = await GetActiveTokensByUserIdAsync(userId, cancellationToken);
            
            foreach (var token in activeTokens)
            {
                token.Revoke(revokedByIp, null, "All tokens revoked");
            }
        }

        public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
        {
            var expiredTokens = await _dbSet
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            _dbSet.RemoveRange(expiredTokens);
        }
    }
}

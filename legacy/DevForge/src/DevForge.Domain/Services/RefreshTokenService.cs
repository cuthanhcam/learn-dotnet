using DevForge.Domain.Entities;
using DevForge.Domain.Exceptions;
using DevForge.Domain.Repositories;

namespace DevForge.Domain.Services
{
    public class RefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, ITokenGenerator tokenGenerator)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<RefreshToken> RotateTokenAsync(string oldToken, string ipAddress, int expirationDays = 7, CancellationToken cancellationToken = default)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(oldToken, cancellationToken);
            
            if (existingToken == null)
                throw new DomainException("Invalid refresh token");

            existingToken.ValidateForUse();

            var newToken = _tokenGenerator.GenerateRefreshToken();
            var newRefreshToken = RefreshToken.Create(
                newToken,
                existingToken.UserId,
                DateTime.UtcNow.AddDays(expirationDays),
                ipAddress
            );

            existingToken.MarkAsUsed(newToken);

            await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            return newRefreshToken;
        }

        public async Task RevokeTokenAsync(string token, string ipAddress, string reason, CancellationToken cancellationToken = default)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken);
            
            if (refreshToken == null)
                throw new DomainException("Token not found");

            refreshToken.Revoke(ipAddress, null, reason);
            await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, ipAddress, cancellationToken);
        }

        public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
        {
            await _refreshTokenRepository.DeleteExpiredTokensAsync(cancellationToken);
        }
    }
}

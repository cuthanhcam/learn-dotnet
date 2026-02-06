using DevForge.Domain.Common;
using DevForge.Domain.Events.RefreshToken;
using DevForge.Domain.Exceptions;

namespace DevForge.Domain.Entities
{
    public class RefreshToken : Entity, IAggregateRoot
    {
        public string Token { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }
        public string? ReplacedByToken { get; private set; }
        public string CreatedByIp { get; private set; }
        public string? DeviceInfo { get; private set; }
        public string? UserAgent { get; private set; }
        public string? ReasonRevoked { get; private set; }
        
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken()
        {
            Token = string.Empty;
            CreatedByIp = string.Empty;
        }

        private RefreshToken(string token, Guid userId, DateTime expiresAt, string createdByIp, string? deviceInfo = null, string? userAgent = null)
        {
            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            CreatedByIp = createdByIp;
            DeviceInfo = deviceInfo;
            UserAgent = userAgent;
            Id = Guid.NewGuid();
        }

        public static RefreshToken Create(string token, Guid userId, DateTime expiresAt, string createdByIp, string? deviceInfo = null, string? userAgent = null)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Token cannot be empty");

            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (expiresAt <= DateTime.UtcNow)
                throw new DomainException("Token expiration must be in the future");

            if (string.IsNullOrWhiteSpace(createdByIp))
                throw new DomainException("Created by IP cannot be empty");

            var refreshToken = new RefreshToken(token, userId, expiresAt, createdByIp, deviceInfo, userAgent);
            refreshToken.AddDomainEvent(new RefreshTokenCreatedEvent(refreshToken.Id, userId, expiresAt));
            return refreshToken;
        }

        public void Revoke(string revokedByIp, string? replacedByToken = null, string? reason = null)
        {
            if (IsRevoked)
                throw new DomainException("Token is already revoked");

            if (string.IsNullOrWhiteSpace(revokedByIp))
                throw new DomainException("Revoked by IP cannot be empty");

            RevokedAt = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
            ReplacedByToken = replacedByToken;
            ReasonRevoked = reason ?? "User requested";

            AddDomainEvent(new RefreshTokenRevokedEvent(Id, UserId, RevokedAt.Value, revokedByIp, ReasonRevoked));
        }

        public void ValidateForUse()
        {
            if (IsRevoked)
                throw new DomainException($"Token has been revoked: {ReasonRevoked}");

            if (IsExpired)
                throw new DomainException("Token has expired");
        }

        public void MarkAsUsed(string newToken)
        {
            Revoke(CreatedByIp, newToken, "Token rotated");
        }
    }
}

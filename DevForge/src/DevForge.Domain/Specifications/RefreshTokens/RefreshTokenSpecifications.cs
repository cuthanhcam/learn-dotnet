using DevForge.Domain.Entities;
using System.Linq.Expressions;

namespace DevForge.Domain.Specifications.RefreshTokens
{
    public class ActiveRefreshTokenSpecification : Specification<RefreshToken>
    {
        public override Expression<Func<RefreshToken, bool>> ToExpression()
        {
            return token => token.RevokedAt == null && token.ExpiresAt > DateTime.UtcNow;
        }
    }

    public class ExpiredRefreshTokenSpecification : Specification<RefreshToken>
    {
        public override Expression<Func<RefreshToken, bool>> ToExpression()
        {
            return token => token.ExpiresAt <= DateTime.UtcNow;
        }
    }

    public class RevokedRefreshTokenSpecification : Specification<RefreshToken>
    {
        public override Expression<Func<RefreshToken, bool>> ToExpression()
        {
            return token => token.RevokedAt != null;
        }
    }

    public class RefreshTokenByUserSpecification : Specification<RefreshToken>
    {
        private readonly Guid _userId;

        public RefreshTokenByUserSpecification(Guid userId)
        {
            _userId = userId;
        }

        public override Expression<Func<RefreshToken, bool>> ToExpression()
        {
            return token => token.UserId == _userId;
        }
    }
}

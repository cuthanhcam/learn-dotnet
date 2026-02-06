using DevForge.Domain.Entities;
using System.Linq.Expressions;

namespace DevForge.Domain.Specifications.Users
{
    public class ActiveUserSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.IsActive;
        }
    }

    public class EmailConfirmedSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.EmailConfirmed;
        }
    }

    public class LockedOutUserSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow;
        }
    }

    public class TwoFactorEnabledSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.TwoFactorEnabled;
        }
    }

    public class UserCanLoginSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.IsActive 
                && (!user.LockoutEnd.HasValue || user.LockoutEnd.Value <= DateTime.UtcNow)
                && (user.EmailConfirmed || !user.LockoutEnabled);
        }
    }

    public class UserByRoleSpecification : Specification<User>
    {
        private readonly Guid _roleId;

        public UserByRoleSpecification(Guid roleId)
        {
            _roleId = roleId;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.UserRoles.Any(ur => ur.RoleId == _roleId);
        }
    }

    public class UserCreatedAfterSpecification : Specification<User>
    {
        private readonly DateTime _date;

        public UserCreatedAfterSpecification(DateTime date)
        {
            _date = date;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.CreatedAt >= _date;
        }
    }

    public class UserLastLoginAfterSpecification : Specification<User>
    {
        private readonly DateTime _date;

        public UserLastLoginAfterSpecification(DateTime date)
        {
            _date = date;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.LastLoginAt.HasValue && user.LastLoginAt.Value >= _date;
        }
    }

    /// <summary>
    /// Specification for users with unconfirmed emails
    /// </summary>
    public class UnconfirmedEmailUsersSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => !user.EmailConfirmed;
        }
    }

    /// <summary>
    /// Specification for searching users by term (username or email)
    /// </summary>
    public class UserSearchSpecification : Specification<User>
    {
        private readonly string _searchTerm;

        public UserSearchSpecification(string searchTerm)
        {
            _searchTerm = searchTerm.ToLower();
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Username.Value.ToLower().Contains(_searchTerm) ||
                          user.Email.Value.ToLower().Contains(_searchTerm);
        }
    }

    /// <summary>
    /// Specification for users created within date range
    /// </summary>
    public class UsersCreatedInRangeSpecification : Specification<User>
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public UsersCreatedInRangeSpecification(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.CreatedAt >= _startDate && user.CreatedAt <= _endDate;
        }
    }

    /// <summary>
    /// Specification for users who never logged in
    /// </summary>
    public class NeverLoggedInUsersSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => !user.LastLoginAt.HasValue;
        }
    }

    /// <summary>
    /// Specification for users with failed login attempts
    /// </summary>
    public class UsersWithFailedAttemptsSpecification : Specification<User>
    {
        private readonly int _minFailedAttempts;

        public UsersWithFailedAttemptsSpecification(int minFailedAttempts = 1)
        {
            _minFailedAttempts = minFailedAttempts;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => user.AccessFailedCount >= _minFailedAttempts;
        }
    }

    /// <summary>
    /// Specification for users requiring password change (password age)
    /// </summary>
    public class PasswordExpiredUsersSpecification : Specification<User>
    {
        private readonly int _maxPasswordAgeDays;

        public PasswordExpiredUsersSpecification(int maxPasswordAgeDays = 90)
        {
            _maxPasswordAgeDays = maxPasswordAgeDays;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_maxPasswordAgeDays);
            return user => user.UpdatedAt.HasValue && user.UpdatedAt.Value <= cutoffDate;
        }
    }

    /// <summary>
    /// Specification for dormant/inactive user accounts
    /// </summary>
    public class DormantUsersSpecification : Specification<User>
    {
        private readonly int _inactiveDays;

        public DormantUsersSpecification(int inactiveDays = 180)
        {
            _inactiveDays = inactiveDays;
        }

        public override Expression<Func<User, bool>> ToExpression()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_inactiveDays);
            return user => user.LastLoginAt.HasValue && user.LastLoginAt.Value <= cutoffDate ||
                          !user.LastLoginAt.HasValue && user.CreatedAt <= cutoffDate;
        }
    }

    /// <summary>
    /// Specification combining multiple conditions for security audit
    /// </summary>
    public class SecurityAuditUsersSpecification : Specification<User>
    {
        public override Expression<Func<User, bool>> ToExpression()
        {
            return user => (!user.EmailConfirmed && user.IsActive) ||
                          (user.AccessFailedCount > 3) ||
                          (user.TwoFactorEnabled == false && user.IsActive);
        }
    }
}

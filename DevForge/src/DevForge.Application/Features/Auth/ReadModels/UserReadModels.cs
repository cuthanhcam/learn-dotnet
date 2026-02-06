namespace DevForge.Application.Features.Auth.ReadModels
{
    /// <summary>
    /// Lightweight read model for user list queries
    /// Optimized for performance - only essential fields
    /// </summary>
    public sealed class UserListReadModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Denormalized for query performance
        public List<string> RoleNames { get; set; } = new();
    }

    /// <summary>
    /// Detailed read model for single user queries
    /// Includes all user information with related data
    /// </summary>
    public sealed class UserDetailReadModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEnd { get; set; }
        
        // Security information
        public SecurityInfoReadModel SecurityInfo { get; set; } = new();
        
        // Denormalized collections
        public List<RoleReadModel> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new(); // Flattened from roles
    }

    public sealed class SecurityInfoReadModel
    {
        public bool IsLockedOut { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastPasswordChangeAt { get; set; }
        public bool RequirePasswordChange { get; set; }
        public int SecurityScore { get; set; }
        public List<string> SecurityWarnings { get; set; } = new();
    }

    public sealed class RoleReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
    }

    public sealed class PermissionReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Read model for authentication/authorization checks
    /// Ultra-lightweight for frequent access
    /// </summary>
    public sealed class UserAuthReadModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsLockedOut { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public List<string> PermissionNames { get; set; } = new();
    }

    /// <summary>
    /// Statistics read model for dashboard/reporting
    /// </summary>
    public sealed class UserStatisticsReadModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int LockedUsers { get; set; }
        public int UsersWithTwoFactor { get; set; }
        public int UsersWithUnconfirmedEmail { get; set; }
        
        public int UsersCreatedToday { get; set; }
        public int UsersCreatedThisWeek { get; set; }
        public int UsersCreatedThisMonth { get; set; }
        
        public int LoginsToday { get; set; }
        public int LoginsThisWeek { get; set; }
        public int LoginsThisMonth { get; set; }
        
        public Dictionary<string, int> UsersByRole { get; set; } = new();
    }
}

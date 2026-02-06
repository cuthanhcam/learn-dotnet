namespace DevForge.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Extended user profile DTO with additional information
    /// </summary>
    public record UserProfileDto(
        Guid Id,
        string Username,
        string Email,
        string? PhoneNumber,
        bool IsActive,
        bool EmailConfirmed,
        bool PhoneNumberConfirmed,
        bool TwoFactorEnabled,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? LastLoginAt,
        int AccessFailedCount,
        DateTime? LockoutEnd,
        bool LockoutEnabled,
        List<RoleDto> Roles,
        List<PermissionDto> Permissions,
        UserSecurityInfoDto SecurityInfo
    );

    /// <summary>
    /// Role information
    /// </summary>
    public record RoleDto(
        Guid Id,
        string Name,
        string? Description
    );

    /// <summary>
    /// Permission information
    /// </summary>
    public record PermissionDto(
        Guid Id,
        string Name,
        string? Description,
        string? Category
    );

    /// <summary>
    /// Detailed role information with permissions
    /// </summary>
    public record RoleDetailDto(
        Guid Id,
        string Name,
        string? Description,
        bool IsSystemRole,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<PermissionDto> Permissions
    );

    /// <summary>
    /// User security information
    /// </summary>
    public record UserSecurityInfoDto(
        bool HasTwoFactorEnabled,
        bool HasEmailConfirmed,
        bool HasPhoneConfirmed,
        int FailedLoginAttempts,
        bool IsLockedOut,
        DateTime? LockoutEndDate,
        DateTime? LastPasswordChangedAt
    );

    /// <summary>
    /// Paginated list result
    /// </summary>
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }

    /// <summary>
    /// User statistics
    /// </summary>
    public record UserStatisticsDto(
        int TotalUsers,
        int ActiveUsers,
        int InactiveUsers,
        int LockedUsers,
        int UsersWithTwoFactor,
        int UsersWithEmailConfirmed,
        int UsersCreatedToday,
        int UsersCreatedThisWeek,
        int UsersCreatedThisMonth
    );
}

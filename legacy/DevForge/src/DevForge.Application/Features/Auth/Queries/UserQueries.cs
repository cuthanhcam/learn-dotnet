using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using MediatR;

namespace DevForge.Application.Features.Auth.Queries
{
    /// <summary>
    /// Query to get user by ID
    /// </summary>
    public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;

    /// <summary>
    /// Query to get current authenticated user
    /// </summary>
    public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserDto>>;

    /// <summary>
    /// Query to get user profile with detailed information
    /// </summary>
    public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;

    /// <summary>
    /// Query to get list of users with pagination
    /// </summary>
    public record GetUsersQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        bool? IsActive = null,
        bool? EmailConfirmed = null,
        string? Role = null
    ) : IRequest<Result<PaginatedList<UserDto>>>;

    /// <summary>
    /// Query to get users by role
    /// </summary>
    public record GetUsersByRoleQuery(
        Guid RoleId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<UserDto>>>;

    /// <summary>
    /// Query to check if username exists
    /// </summary>
    public record CheckUsernameExistsQuery(string Username) : IRequest<Result<bool>>;

    /// <summary>
    /// Query to check if email exists
    /// </summary>
    public record CheckEmailExistsQuery(string Email) : IRequest<Result<bool>>;

    /// <summary>
    /// Query to get user statistics
    /// </summary>
    public record GetUserStatisticsQuery : IRequest<Result<UserStatisticsDto>>;
}

using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevForge.API.Controllers
{
    /// <summary>
    /// User management endpoints
    /// </summary>
    /// <remarks>
    /// Provides comprehensive user management features including:
    /// - User listing with pagination and filtering
    /// - User profile retrieval
    /// - User statistics and analytics
    /// - Username and email availability checks
    /// - Role-based user queries
    /// </remarks>
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the UsersController
        /// </summary>
        /// <param name="mediator">MediatR instance for CQRS pattern</param>
        /// <param name="logger">Logger instance</param>
        public UsersController(ISender mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of users with optional filtering
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users?pageNumber=1&amp;pageSize=10&amp;searchTerm=john&amp;isActive=true
        /// 
        /// Search term filters by username or email.
        /// Optional filters: isActive, emailConfirmed.
        /// Results are paginated for performance.
        /// </remarks>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search by username or email</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="emailConfirmed">Filter by email confirmation status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Successfully retrieved users</response>
        /// <response code="400">Invalid query parameters</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpGet]
        [Authorize(Policy = "users.read")]
        [ProducesResponseType(typeof(Result<PaginatedList<UserProfileDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? emailConfirmed = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsActive = isActive,
                EmailConfirmed = emailConfirmed
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// 
        /// Returns detailed user information including roles and permissions.
        /// </remarks>
        /// <param name="userId">User unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User details</returns>
        /// <response code="200">User found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">User not found</response>
        [HttpGet("{userId:guid}")]
        [Authorize(Policy = "users.read")]
        [ProducesResponseType(typeof(Result<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Get current authenticated user profile
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/me
        /// 
        /// Returns profile information for the currently authenticated user.
        /// Extracts user ID from JWT token automatically.
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Current user profile</returns>
        /// <response code="200">Successfully retrieved profile</response>
        /// <response code="401">Unauthorized or invalid token</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(Result<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Failed to extract user ID from token");
                return Unauthorized(Result<UserProfileDto>.Failure(Error.Unauthorized("Auth.InvalidToken", "User ID not found in token")));
            }

            var query = new GetCurrentUserQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return Unauthorized(result);
        }

        /// <summary>
        /// Get detailed user profile with security information
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/3fa85f64-5717-4562-b3fc-2c963f66afa6/profile
        /// 
        /// Returns extended profile including:
        /// - Basic user information
        /// - Security settings (2FA status, account lockout)
        /// - Last login information
        /// - Assigned roles and permissions
        /// </remarks>
        /// <param name="userId">User unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed user profile</returns>
        /// <response code="200">Profile retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">User not found</response>
        [HttpGet("{userId:guid}/profile")]
        [Authorize(Policy = "users.read")]
        [ProducesResponseType(typeof(Result<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfile(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Get users assigned to a specific role
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/roles/3fa85f64-5717-4562-b3fc-2c963f66afa6?pageNumber=1&amp;pageSize=20
        /// 
        /// Returns paginated list of users who have the specified role assigned.
        /// Useful for role membership auditing and management.
        /// </remarks>
        /// <param name="roleId">Role unique identifier</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of users with the role</returns>
        /// <response code="200">Successfully retrieved users</response>
        /// <response code="400">Invalid query parameters</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpGet("roles/{roleId:guid}")]
        [Authorize(Policy = "users.read")]
        [ProducesResponseType(typeof(Result<PaginatedList<UserProfileDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsersByRole(
            Guid roleId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUsersByRoleQuery(roleId, pageNumber, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Check if username is available
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/check-username/johndoe
        /// 
        /// Returns true if username is already taken, false if available.
        /// Used for real-time validation during registration.
        /// </remarks>
        /// <param name="username">Username to check</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Boolean indicating if username exists</returns>
        /// <response code="200">Check completed successfully</response>
        /// <response code="400">Invalid username format</response>
        [HttpGet("check-username/{username}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckUsernameExists(
            string username,
            CancellationToken cancellationToken = default)
        {
            var query = new CheckUsernameExistsQuery(username);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Check if email is available
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/check-email/john@example.com
        /// 
        /// Returns true if email is already registered, false if available.
        /// Used for real-time validation during registration.
        /// </remarks>
        /// <param name="email">Email address to check</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Boolean indicating if email exists</returns>
        /// <response code="200">Check completed successfully</response>
        /// <response code="400">Invalid email format</response>
        [HttpGet("check-email/{email}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckEmailExists(
            string email,
            CancellationToken cancellationToken = default)
        {
            var query = new CheckEmailExistsQuery(email);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get user statistics for dashboard
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/statistics
        /// 
        /// Returns comprehensive user metrics including:
        /// - Total users count
        /// - Active/inactive users
        /// - Users with 2FA enabled
        /// - Recent registrations (today, this week, this month)
        /// - Recent logins
        /// - Users by role breakdown
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpGet("statistics")]
        [Authorize(Policy = "users.read")]
        [ProducesResponseType(typeof(Result<UserStatisticsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserStatistics(CancellationToken cancellationToken = default)
        {
            var query = new GetUserStatisticsQuery();
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

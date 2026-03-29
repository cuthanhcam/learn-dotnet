using DevForge.Application.Features.Auth.Commands;
using DevForge.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevForge.API.Controllers
{
    /// <summary>
    /// Permission management endpoints
    /// </summary>
    /// <remarks>
    /// Provides permission management features including:
    /// - Permission listing and filtering
    /// - Permission retrieval by ID or category
    /// - Permission creation (admin only)
    /// - Category-based organization
    /// </remarks>
    [ApiController]
    [Route("api/v1/permissions")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly ILogger<PermissionsController> _logger;
        private readonly ISender _mediator;

        /// <summary>
        /// Initializes a new instance of the PermissionsController
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="mediator">MediatR instance for CQRS pattern</param>
        public PermissionsController(ILogger<PermissionsController> logger, ISender mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Get all permissions with optional category filter
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/permissions?category=users
        /// 
        /// Returns all permissions in the system, optionally filtered by category.
        /// 
        /// Common categories:
        /// - users: User management permissions
        /// - roles: Role management permissions
        /// - permissions: Permission management permissions
        /// - content: Content management permissions
        /// </remarks>
        /// <param name="category">Filter by permission category (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of permissions</returns>
        /// <response code="200">Successfully retrieved permissions</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpGet]
        [Authorize(Policy = "permissions.read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPermissions(
            [FromQuery] string? category = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving permissions with category filter: {Category}", category ?? "all");
            var query = new GetPermissionsQuery(category);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get permission by ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/permissions/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// 
        /// Returns detailed permission information including:
        /// - Permission name (e.g., "users.create", "roles.read")
        /// - Category
        /// - Description
        /// - Roles that have this permission
        /// </remarks>
        /// <param name="permissionId">Permission unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Permission details</returns>
        /// <response code="200">Permission found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Permission not found</response>
        [HttpGet("{permissionId:guid}")]
        [Authorize(Policy = "permissions.read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissionById(
            Guid permissionId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving permission {PermissionId}", permissionId);
            var query = new GetPermissionByIdQuery(permissionId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Get permissions by category
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/permissions/categories/users
        /// 
        /// Returns all permissions belonging to a specific category.
        /// Useful for permission management UI grouped by feature area.
        /// 
        /// Example categories and their permissions:
        /// - users: users.read, users.create, users.update, users.delete
        /// - roles: roles.read, roles.create, roles.update, roles.delete
        /// - permissions: permissions.read, permissions.create
        /// </remarks>
        /// <param name="category">Permission category name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of permissions in the category</returns>
        /// <response code="200">Successfully retrieved permissions</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpGet("categories/{category}")]
        [Authorize(Policy = "permissions.read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPermissionsByCategory(
            string category,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving permissions for category: {Category}", category);
            var query = new GetPermissionsQuery(category);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Create new permission
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/permissions
        ///     {
        ///        "name": "content.publish",
        ///        "category": "content",
        ///        "description": "Can publish content to production"
        ///     }
        /// 
        /// Permission naming convention: {category}.{action}
        /// - category: Feature area (users, roles, content, etc.)
        /// - action: Operation (read, create, update, delete, publish, etc.)
        /// 
        /// Examples:
        /// - users.read, users.create, users.delete
        /// - content.publish, content.archive
        /// - reports.generate, reports.export
        /// </remarks>
        /// <param name="request">Permission creation details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created permission</returns>
        /// <response code="201">Permission successfully created</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="409">Permission name already exists</response>
        [HttpPost]
        [Authorize(Policy = "permissions.create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePermission(
            [FromBody] CreatePermissionRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating new permission: {PermissionName}", request.Name);
            var command = new CreatePermissionCommand(request.Name, request.Category, request.Description);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Code.Contains("Conflict") == true)
                    return Conflict(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error?.Message });
            }

            return CreatedAtAction(nameof(GetPermissionById), new { permissionId = result.Data }, result);
        }

        /// <summary>
        /// Update permission
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/permissions/3fa85f64-5717-4562-b3fc-2c963f66afa6
        ///     {
        ///        "name": "content.publish",
        ///        "category": "content",
        ///        "description": "Can publish and schedule content to production"
        ///     }
        /// 
        /// Permission name must remain unique.
        /// Changing permission name affects all roles using it.
        /// </remarks>
        /// <param name="permissionId">Permission unique identifier</param>
        /// <param name="request">Updated permission details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        /// <response code="204">Permission successfully updated</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Permission not found</response>
        /// <response code="409">Permission name already exists</response>
        [HttpPut("{permissionId:guid}")]
        [Authorize(Policy = "permissions.update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePermission(
            Guid permissionId,
            [FromBody] UpdatePermissionRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating permission {PermissionId}", permissionId);
            var command = new UpdatePermissionCommand(permissionId, request.Name, request.Category, request.Description);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Code.Contains("NotFound") == true)
                    return NotFound(new { error = result.Error.Message });

                if (result.Error?.Code.Contains("Conflict") == true)
                    return Conflict(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error?.Message });
            }

            return NoContent();
        }

        /// <summary>
        /// Delete permission
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/permissions/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// 
        /// Removes permission from all roles that have it assigned.
        /// Use with caution as this affects access control across the system.
        /// </remarks>
        /// <param name="permissionId">Permission unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        /// <response code="204">Permission successfully deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Permission not found</response>
        [HttpDelete("{permissionId:guid}")]
        [Authorize(Policy = "permissions.delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePermission(
            Guid permissionId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting permission {PermissionId}", permissionId);
            var command = new DeletePermissionCommand(permissionId);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Code.Contains("NotFound") == true)
                    return NotFound(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error?.Message });
            }

            return NoContent();
        }
    }

    /// <summary>
    /// Request model for creating a new permission
    /// </summary>
    /// <param name="Name">Permission name in format: category.action (e.g., "users.create")</param>
    /// <param name="Category">Permission category (e.g., "users", "roles", "content")</param>
    /// <param name="Description">Permission description (optional)</param>
    public record CreatePermissionRequest(string Name, string Category, string? Description);

    /// <summary>
    /// Request model for updating an existing permission
    /// </summary>
    /// <param name="Name">Updated permission name in format: category.action</param>
    /// <param name="Category">Updated permission category</param>
    /// <param name="Description">Updated permission description (optional)</param>
    public record UpdatePermissionRequest(string Name, string Category, string? Description);
}

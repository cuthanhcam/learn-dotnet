using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.Commands;
using DevForge.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevForge.API.Controllers
{
    /// <summary>
    /// Role management endpoints
    /// </summary>
    /// <remarks>
    /// Provides comprehensive role management features including:
    /// - Role CRUD operations
    /// - Permission assignment to roles
    /// - Role hierarchy management
    /// - System role protection
    /// </remarks>
    [ApiController]
    [Route("api/v1/roles")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<RolesController> _logger;

        /// <summary>
        /// Initializes a new instance of the RolesController
        /// </summary>
        /// <param name="mediator">MediatR instance for CQRS pattern</param>
        /// <param name="logger">Logger instance</param>
        public RolesController(ISender mediator, ILogger<RolesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/roles
        /// 
        /// Returns all roles in the system including system roles and custom roles.
        /// System roles (Admin, User, etc.) cannot be deleted.
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all roles</returns>
        /// <response code="200">Successfully retrieved roles</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpGet]
        [Authorize(Policy = "roles.read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all roles");
            var query = new GetRolesQuery();
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/roles/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// 
        /// Returns detailed role information including:
        /// - Role name and description
        /// - System role indicator
        /// - Assigned permissions
        /// - User count
        /// </remarks>
        /// <param name="roleId">Role unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Role details</returns>
        /// <response code="200">Role found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{roleId:guid}")]
        [Authorize(Policy = "roles.read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoleById(Guid roleId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving role {RoleId}", roleId);
            var query = new GetRoleByIdQuery(roleId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Create new role
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/roles
        ///     {
        ///        "name": "Content Manager",
        ///        "description": "Can manage content and posts"
        ///     }
        /// 
        /// Role name must be unique (case-insensitive).
        /// Created roles are not system roles by default.
        /// Permissions can be assigned after creation.
        /// </remarks>
        /// <param name="request">Role creation details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created role</returns>
        /// <response code="201">Role successfully created</response>
        /// <response code="400">Invalid input or validation error</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="409">Role name already exists</response>
        [HttpPost]
        [Authorize(Policy = "roles.create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateRole(
            [FromBody] CreateRoleRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating new role: {RoleName}", request.Name);
            var command = new CreateRoleCommand(request.Name, request.Description);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Code.Contains("Conflict") == true)
                    return Conflict(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error?.Message });
            }

            return CreatedAtAction(nameof(GetRoleById), new { roleId = result.Data }, result);
        }

        /// <summary>
        /// Update role
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/roles/3fa85f64-5717-4562-b3fc-2c963f66afa6
        ///     {
        ///        "name": "Senior Content Manager",
        ///        "description": "Can manage all content and approve posts"
        ///     }
        /// 
        /// System roles cannot be renamed or modified.
        /// Role name must remain unique.
        /// </remarks>
        /// <param name="roleId">Role unique identifier</param>
        /// <param name="request">Updated role details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        /// <response code="204">Role successfully updated</response>
        /// <response code="400">Invalid input or system role modification attempt</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Role not found</response>
        /// <response code="409">Role name already exists</response>
        [HttpPut("{roleId:guid}")]
        [Authorize(Policy = "roles.update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateRole(
            Guid roleId,
            [FromBody] UpdateRoleRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating role {RoleId}", roleId);
            var command = new UpdateRoleCommand(roleId, request.Name, request.Description);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Code.Contains("NotFound") == true)
                    return NotFound(new { error = result.Error.Message });

                if (result.Error?.Code.Contains("Conflict") == true)
                    return BadRequest(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error?.Message });
            }

            return NoContent();
        }

        /// <summary>
        /// Assign permissions to role
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/roles/3fa85f64-5717-4562-b3fc-2c963f66afa6/permissions
        ///     {
        ///        "permissionIds": [
        ///          "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///          "5fc72d89-1234-5678-9abc-def012345678"
        ///        ]
        ///     }
        /// 
        /// Replaces all existing permissions with the provided list.
        /// To add permissions, include existing ones in the list.
        /// Empty array removes all permissions.
        /// </remarks>
        /// <param name="roleId">Role unique identifier</param>
        /// <param name="request">List of permission IDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        /// <response code="204">Permissions successfully assigned</response>
        /// <response code="400">Invalid permission IDs</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Role or permission not found</response>
        [HttpPost("{roleId:guid}/permissions")]
        [Authorize(Policy = "roles.update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignPermissions(
            Guid roleId,
            [FromBody] AssignPermissionsRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Assigning {Count} permissions to role {RoleId}", request.PermissionIds.Count, roleId);
            var command = new AssignPermissionsToRoleCommand(roleId, request.PermissionIds);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Code.Contains("NotFound") == true)
                    return NotFound(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error?.Message });
            }

            return NoContent();
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/roles/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// 
        /// System roles (Admin, User, etc.) cannot be deleted.
        /// Users assigned to the role will have it removed.
        /// Associated permissions remain in the system.
        /// </remarks>
        /// <param name="roleId">Role unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        /// <response code="204">Role successfully deleted</response>
        /// <response code="400">System role deletion attempt</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Insufficient permissions</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{roleId:guid}")]
        [Authorize(Policy = "roles.delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRole(Guid roleId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting role {RoleId}", roleId);
            var command = new DeleteRoleCommand(roleId);
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
    /// Request model for creating a new role
    /// </summary>
    /// <param name="Name">Role name (unique, 3-50 characters)</param>
    /// <param name="Description">Role description (optional)</param>
    public record CreateRoleRequest(string Name, string? Description);

    /// <summary>
    /// Request model for updating an existing role
    /// </summary>
    /// <param name="Name">Updated role name (unique, 3-50 characters)</param>
    /// <param name="Description">Updated role description (optional)</param>
    public record UpdateRoleRequest(string Name, string? Description);

    /// <summary>
    /// Request model for assigning permissions to a role
    /// </summary>
    /// <param name="PermissionIds">List of permission unique identifiers</param>
    public record AssignPermissionsRequest(List<Guid> PermissionIds);
}

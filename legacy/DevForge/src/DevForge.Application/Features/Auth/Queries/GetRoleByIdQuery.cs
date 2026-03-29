using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Repositories;
using MediatR;

namespace DevForge.Application.Features.Auth.Queries
{
    public record GetRoleByIdQuery(Guid RoleId) : IRequest<Result<RoleDetailDto>>;

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDetailDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public GetRoleByIdQueryHandler(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<Result<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result<RoleDetailDto>.Failure(Error.NotFound("Role.NotFound", "Role not found"));

            // Get permission details
            var permissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
            
            var permissionDtos = permissions
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => new PermissionDto(p.Id, p.Name, p.Description, p.Category))
                .ToList();

            var roleDto = new RoleDetailDto(
                role.Id,
                role.Name,
                role.Description,
                role.IsSystemRole,
                role.CreatedAt,
                null, // Role doesn't have UpdatedAt
                permissionDtos
            );

            return Result<RoleDetailDto>.Success(roleDto);
        }
    }
}

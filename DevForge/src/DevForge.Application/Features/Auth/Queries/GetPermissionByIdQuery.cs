using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Repositories;
using MediatR;

namespace DevForge.Application.Features.Auth.Queries
{
    public record GetPermissionByIdQuery(Guid PermissionId) : IRequest<Result<PermissionDto>>;

    public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, Result<PermissionDto>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionByIdQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Result<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
            if (permission == null)
                return Result<PermissionDto>.Failure(Error.NotFound("Role.NotFound", "Permission not found"));

            var permissionDto = new PermissionDto(
                permission.Id,
                permission.Name,
                permission.Description,
                permission.Category
            );

            return Result<PermissionDto>.Success(permissionDto);
        }
    }
}

using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Repositories;
using MediatR;

namespace DevForge.Application.Features.Auth.Queries
{
    public record GetPermissionsQuery(string? Category = null) : IRequest<Result<List<PermissionDto>>>;

    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<List<PermissionDto>>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionsQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

            // Filter by category if provided
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                permissions = permissions.Where(p => p.Category == request.Category);
            }
            
            var permissionDtos = permissions
                .Select(p => new PermissionDto(p.Id, p.Name, p.Description, p.Category))
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToList();

            return Result<List<PermissionDto>>.Success(permissionDtos);
        }
    }
}

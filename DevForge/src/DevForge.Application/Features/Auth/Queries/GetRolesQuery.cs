using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Repositories;
using MediatR;

namespace DevForge.Application.Features.Auth.Queries
{
    public record GetRolesQuery : IRequest<Result<List<RoleDto>>>;

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync(cancellationToken);
            
            var roleDtos = roles
                .Select(r => new RoleDto(r.Id, r.Name, r.Description))
                .OrderBy(r => r.Name)
                .ToList();

            return Result<List<RoleDto>>.Success(roleDtos);
        }
    }
}

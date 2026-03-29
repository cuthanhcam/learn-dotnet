using DevForge.Application.Common.Models;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record DeleteRoleCommand(Guid RoleId) : IRequest<Result<bool>>;

    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required");
        }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                return Result<bool>.Failure(Error.NotFound("Role.NotFound", "Role not found"));
            }

            // Prevent deletion of system roles
            if (role.IsSystemRole)
            {
                return Result<bool>.Failure(Error.NotFound("Role.NotFound", "Cannot delete system roles"));
            }

            // Delete role (cascade will remove user-role assignments)
            await _roleRepository.DeleteAsync(role, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}

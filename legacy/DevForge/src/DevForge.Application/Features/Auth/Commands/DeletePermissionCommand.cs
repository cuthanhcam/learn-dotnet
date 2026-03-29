using DevForge.Application.Common.Models;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record DeletePermissionCommand(Guid PermissionId) : IRequest<Result<bool>>;

    public class DeletePermissionCommandValidator : AbstractValidator<DeletePermissionCommand>
    {
        public DeletePermissionCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("Permission ID is required");
        }
    }

    public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, Result<bool>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public DeletePermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Result<bool>> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
            if (permission == null)
            {
                return Result<bool>.Failure(Error.NotFound("Role.NotFound", "Permission not found"));
            }

            // Delete permission (cascade will remove role-permission assignments)
            await _permissionRepository.DeleteAsync(permission, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}

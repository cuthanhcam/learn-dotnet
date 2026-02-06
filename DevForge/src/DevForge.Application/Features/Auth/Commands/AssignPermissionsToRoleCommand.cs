using DevForge.Application.Common.Models;
using DevForge.Domain.Common;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record AssignPermissionsToRoleCommand(
        Guid RoleId,
        List<Guid> PermissionIds
    ) : IRequest<Result>;

    public class AssignPermissionsToRoleCommandValidator : AbstractValidator<AssignPermissionsToRoleCommand>
    {
        public AssignPermissionsToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required");

            RuleFor(x => x.PermissionIds)
                .NotNull().WithMessage("Permission IDs cannot be null")
                .Must(ids => ids != null && ids.Count > 0).WithMessage("At least one permission must be specified");
        }
    }

    public class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, Result>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignPermissionsToRoleCommandHandler(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result.Failure(Error.Failure("Error.General", "Role not found"));

            // Validate all permissions exist
            var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
            var permissionList = permissions.Where(p => request.PermissionIds.Contains(p.Id)).ToList();
            
            if (permissionList.Count != request.PermissionIds.Count)
                return Result.Failure(Error.Failure("Error.General", "One or more permissions not found"));

            // Remove all existing permissions and add new ones
            var existingPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            foreach (var permissionId in existingPermissionIds)
            {
                role.RemovePermission(permissionId);
            }

            foreach (var permission in permissionList)
            {
                role.AddPermission(permission);
            }

            await _roleRepository.UpdateAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

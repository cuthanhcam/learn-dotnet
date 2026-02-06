using DevForge.Application.Common.Models;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record UpdateRoleCommand(Guid RoleId, string Name, string? Description) : IRequest<Result<bool>>;

    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required")
                .Length(3, 50).WithMessage("Role name must be between 3 and 50 characters")
                .Matches("^[a-zA-Z0-9 _-]+$").WithMessage("Role name can only contain letters, numbers, spaces, hyphens and underscores");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<bool>>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Result<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                return Result<bool>.Failure(Error.NotFound("Role.NotFound", "Role not found"));
            }

            // Prevent modification of system roles
            if (role.IsSystemRole)
            {
                return Result<bool>.Failure(Error.NotFound("Role.NotFound", "Cannot modify system roles"));
            }

            // Check if new name conflicts with existing role
            if (role.Name != request.Name)
            {
                var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);
                if (existingRole != null && existingRole.Id != request.RoleId)
                {
                    return Result<bool>.Failure(Error.Conflict("Role.NameConflict", $"Role with name '{request.Name}' already exists"));
                }
            }

            // Update role
            role.UpdateDetails(request.Name, request.Description);
            await _roleRepository.UpdateAsync(role, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}

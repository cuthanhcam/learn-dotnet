using DevForge.Application.Common.Models;
using DevForge.Domain.Common;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record UpdatePermissionCommand(
        Guid PermissionId,
        string Name,
        string? Description,
        string? Category
    ) : IRequest<Result>;

    public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
    {
        public UpdatePermissionCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("Permission ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Permission name is required")
                .Length(3, 100).WithMessage("Permission name must be between 3 and 100 characters")
                .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Permission name can only contain letters, numbers, dots, hyphens and underscores");

            RuleFor(x => x.Category)
                .Length(2, 50).WithMessage("Category must be between 2 and 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Category));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, Result>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePermissionCommandHandler(
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
            if (permission == null)
                return Result.Failure(Error.Failure("Error.General", "Permission not found"));

            // Check if another permission with same name exists
            var existingPermission = await _permissionRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingPermission != null && existingPermission.Id != request.PermissionId)
                return Result.Failure(Error.Conflict("Permission.NameConflict", $"Permission with name '{request.Name}' already exists"));

            // UpdateDetails only accepts description and category (name cannot be changed)
            permission.UpdateDetails(request.Description ?? string.Empty, request.Category ?? string.Empty);

            await _permissionRepository.UpdateAsync(permission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

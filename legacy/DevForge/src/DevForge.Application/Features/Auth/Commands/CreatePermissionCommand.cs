using DevForge.Application.Common.Models;
using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record CreatePermissionCommand(string Name, string Category, string? Description) : IRequest<Result<Guid>>;

    public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Permission name is required")
                .Length(3, 100).WithMessage("Permission name must be between 3 and 100 characters")
                .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Permission name can only contain letters, numbers, dots, hyphens and underscores");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required")
                .Length(2, 50).WithMessage("Category must be between 2 and 50 characters")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Category can only contain letters, numbers, hyphens and underscores");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, Result<Guid>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public CreatePermissionCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Result<Guid>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            // Check if permission name already exists
            var existingPermission = await _permissionRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingPermission != null)
            {
                return Result<Guid>.Failure(Error.Conflict("Permission.AlreadyExists", $"Permission with name '{request.Name}' already exists"));
            }

            // Create new permission
            var permission = Permission.Create(request.Name, request.Category, request.Description);
            await _permissionRepository.AddAsync(permission, cancellationToken);

            return Result<Guid>.Success(permission.Id);
        }
    }
}

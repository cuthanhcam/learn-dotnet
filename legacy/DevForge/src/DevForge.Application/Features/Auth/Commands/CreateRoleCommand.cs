using DevForge.Application.Common.Models;
using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace DevForge.Application.Features.Auth.Commands
{
    public record CreateRoleCommand(string Name, string? Description) : IRequest<Result<Guid>>;

    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required")
                .Length(3, 50).WithMessage("Role name must be between 3 and 50 characters")
                .Matches("^[a-zA-Z0-9 _-]+$").WithMessage("Role name can only contain letters, numbers, spaces, hyphens and underscores");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
    {
        private readonly IRoleRepository _roleRepository;

        public CreateRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // Check if role name already exists
            var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingRole != null)
            {
                return Result<Guid>.Failure(Error.Conflict("Role.AlreadyExists", $"Role with name '{request.Name}' already exists"));
            }

            // Create new role
            var role = Role.Create(request.Name, request.Description);
            await _roleRepository.AddAsync(role, cancellationToken);

            return Result<Guid>.Success(role.Id);
        }
    }
}

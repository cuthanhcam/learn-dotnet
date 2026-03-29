using DevForge.Application.Common.Errors;
using DevForge.Application.Common.Extensions;
using DevForge.Application.Common.Interfaces;
using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.Commands;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Common;
using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using DevForge.Domain.Services;
using DevForge.Domain.ValueObjects;
using MediatR;

namespace DevForge.Application.Features.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenticationTokenProvider _tokenProvider;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator,
            IAuthenticationTokenProvider tokenProvider,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _tokenProvider = tokenProvider;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var username = Username.Create(request.Username);
                var email = Email.Create(request.Email);

                if (await _userRepository.ExistsByUsernameAsync(username, cancellationToken))
                    return Result<AuthResponse>.Failure(AuthErrors.UsernameAlreadyExists);

                if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
                    return Result<AuthResponse>.Failure(AuthErrors.EmailAlreadyExists);

                var password = Password.Create(request.Password);
                var passwordHash = _passwordHasher.HashPassword(password);

                var user = User.Create(username, email, passwordHash);

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
                    user.UpdateProfile(phoneNumber: phoneNumber);
                }

                var defaultRole = await _roleRepository.GetByNameAsync(Domain.Constants.Roles.User, cancellationToken);
                if (defaultRole != null)
                {
                    user.AssignRole(defaultRole.Id);
                }

                var emailConfirmationToken = _tokenGenerator.GenerateEmailConfirmationToken();
                user.GenerateEmailConfirmationToken(emailConfirmationToken);

                await _userRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Send confirmation email (fire and forget)
                var confirmationLink = $"https://yourapp.com/confirm-email?email={email.Value}&token={emailConfirmationToken}";
                _ = _notificationService.SendEmailConfirmationAsync(email.Value, username.Value, confirmationLink, cancellationToken);

                var roles = defaultRole != null ? new List<string> { defaultRole.Name } : new List<string>();
                var permissions = defaultRole != null 
                    ? (await _permissionRepository.GetPermissionsByRoleIdAsync(defaultRole.Id, cancellationToken)).Select(p => p.Name).ToList()
                    : new List<string>();

                var accessToken = _tokenProvider.GenerateAccessToken(user.Id, username.Value, email.Value, roles, permissions);

                // Use mapping extension to create response
                var response = user.ToAuthResponse(
                    accessToken,
                    string.Empty, // No refresh token on registration
                    DateTime.UtcNow.AddHours(1),
                    roles,
                    permissions
                );

                return Result<AuthResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(
                    Error.Failure("Auth.RegistrationFailed", $"Registration failed: {ex.Message}"));
            }
        }
    }
}

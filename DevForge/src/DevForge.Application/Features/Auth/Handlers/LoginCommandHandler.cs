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
using MediatR;

namespace DevForge.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserAuthenticationService _authenticationService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenticationTokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IUserAuthenticationService authenticationService,
            IRefreshTokenRepository refreshTokenRepository,
            IPermissionRepository permissionRepository,
            ITokenGenerator tokenGenerator,
            IAuthenticationTokenProvider tokenProvider,
            IUnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;
            _refreshTokenRepository = refreshTokenRepository;
            _permissionRepository = permissionRepository;
            _tokenGenerator = tokenGenerator;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Use Domain Service for authentication
                var user = await _authenticationService.AuthenticateAsync(
                    request.UsernameOrEmail,
                    request.Password,
                    cancellationToken);

                // Get user roles and permissions
                var roles = new List<string>();
                var permissions = new List<string>();

                foreach (var userRole in user.UserRoles)
                {
                    var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(userRole.RoleId, cancellationToken);
                    if (role != null)
                    {
                        roles.Add(role.Name);
                        var rolePermissions = await _permissionRepository.GetPermissionsByRoleIdAsync(role.Id, cancellationToken);
                        permissions.AddRange(rolePermissions.Select(p => p.Name));
                    }
                }

                permissions = permissions.Distinct().ToList();

                // Generate tokens
                var accessToken = _tokenProvider.GenerateAccessToken(user.Id, user.Username.Value, user.Email.Value, roles, permissions);
                var refreshTokenValue = _tokenGenerator.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(request.RememberMe ? 30 : 7);
                var refreshToken = RefreshToken.Create(refreshTokenValue, user.Id, refreshTokenExpiry, request.IpAddress ?? "unknown");

                await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Use mapping extension to create response
                var response = user.ToAuthResponse(
                    accessToken,
                    refreshTokenValue,
                    DateTime.UtcNow.AddHours(1),
                    roles,
                    permissions
                );

                return Result<AuthResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(
                    Error.Failure("Auth.LoginFailed", $"Login failed: {ex.Message}"));
            }
        }
    }
}

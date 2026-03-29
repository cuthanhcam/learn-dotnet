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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenticationTokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPermissionRepository permissionRepository,
            ITokenGenerator tokenGenerator,
            IAuthenticationTokenProvider tokenProvider,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _permissionRepository = permissionRepository;
            _tokenGenerator = tokenGenerator;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
                
                if (existingToken == null)
                    return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);

                existingToken.ValidateForUse();

                var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
                if (user == null)
                    return Result<AuthResponse>.Failure(AuthErrors.UserNotFound);

                if (!user.CanLogin())
                    return Result<AuthResponse>.Failure(AuthErrors.UserInactive);

                var newTokenValue = _tokenGenerator.GenerateRefreshToken();
                var newRefreshToken = RefreshToken.Create(
                    newTokenValue,
                    user.Id,
                    DateTime.UtcNow.AddDays(7),
                    request.IpAddress ?? "unknown"
                );

                existingToken.MarkAsUsed(newTokenValue);

                await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);
                await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

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

                var accessToken = _tokenProvider.GenerateAccessToken(user.Id, user.Username.Value, user.Email.Value, roles, permissions);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Use mapping extension to create response
                var response = user.ToAuthResponse(
                    accessToken,
                    newTokenValue,
                    DateTime.UtcNow.AddHours(1),
                    roles,
                    permissions
                );

                return Result<AuthResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(
                    Error.Failure("Auth.TokenRefreshFailed", $"Token refresh failed: {ex.Message}"));
            }
        }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
                if (token != null && token.IsActive)
                {
                    token.Revoke(request.IpAddress ?? "unknown", null, "User logout");
                    await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.LogoutFailed", $"Logout failed: {ex.Message}"));
            }
        }
    }
}

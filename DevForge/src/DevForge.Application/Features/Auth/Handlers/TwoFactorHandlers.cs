using DevForge.Application.Common.Interfaces;
using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.Commands;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Common;
using DevForge.Domain.Repositories;
using DevForge.Domain.Services;
using MediatR;

namespace DevForge.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handler for enabling two-factor authentication
    /// </summary>
    public class EnableTwoFactorCommandHandler : IRequestHandler<EnableTwoFactorCommand, Result<TwoFactorSetupResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IUnitOfWork _unitOfWork;

        public EnableTwoFactorCommandHandler(
            IUserRepository userRepository,
            ITwoFactorService twoFactorService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _twoFactorService = twoFactorService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TwoFactorSetupResponse>> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return Result<TwoFactorSetupResponse>.Failure(Error.NotFound("Role.NotFound", "User not found"));

                if (user.TwoFactorEnabled)
                    return Result<TwoFactorSetupResponse>.Failure(Error.NotFound("Role.NotFound", "Two-factor authentication is already enabled"));

                // Generate secret key
                var secretKey = _twoFactorService.GenerateSecretKey();
                var qrCodeUri = _twoFactorService.GenerateQrCodeUri(user.Email.Value, secretKey);

                user.EnableTwoFactor(secretKey);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var response = new TwoFactorSetupResponse(secretKey, qrCodeUri);
                return Result<TwoFactorSetupResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<TwoFactorSetupResponse>.Failure(
                    Error.Failure("Auth.Enable2FAFailed", $"Failed to enable 2FA: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Handler for disabling two-factor authentication
    /// </summary>
    public class DisableTwoFactorCommandHandler : IRequestHandler<DisableTwoFactorCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DisableTwoFactorCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return Result.Failure(Error.Failure("Error.General", "User not found"));

                if (!user.TwoFactorEnabled)
                    return Result.Failure(Error.Failure("Error.General", "Two-factor authentication is not enabled"));

                user.DisableTwoFactor();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.Disable2FAFailed", $"Failed to disable 2FA: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Handler for verifying two-factor code
    /// </summary>
    public class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyTwoFactorCommandHandler(
            IUserRepository userRepository,
            ITwoFactorService twoFactorService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _twoFactorService = twoFactorService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return Result.Failure(Error.Failure("Error.General", "User not found"));

                if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecretKey))
                    return Result.Failure(Error.Failure("Error.General", "Two-factor authentication is not enabled"));

                var isValid = _twoFactorService.ValidateCode(user.TwoFactorSecretKey, request.Code);
                if (!isValid)
                    return Result.Failure(Error.Failure("Error.General", "Invalid verification code"));

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.VerifyCodeFailed", $"Failed to verify code: {ex.Message}"));
            }
        }
    }
}

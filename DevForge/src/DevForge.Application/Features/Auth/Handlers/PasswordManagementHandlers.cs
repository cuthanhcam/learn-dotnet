using DevForge.Application.Common.Errors;
using DevForge.Application.Common.Interfaces;
using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.Commands;
using DevForge.Domain.Common;
using DevForge.Domain.Repositories;
using DevForge.Domain.Services;
using DevForge.Domain.ValueObjects;
using MediatR;

namespace DevForge.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handler for change password command
    /// </summary>
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return Result.Failure(AuthErrors.UserNotFound);

                var currentPassword = Password.Create(request.CurrentPassword);
                if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                    return Result.Failure(AuthErrors.InvalidCredentials);

                var newPassword = Password.Create(request.NewPassword);
                var newPasswordHash = _passwordHasher.HashPassword(newPassword);

                user.ChangePassword(newPasswordHash);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.ChangePasswordFailed", $"Failed to change password: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Handler for forgot password command
    /// </summary>
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var email = Email.Create(request.Email);
                var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

                // Don't reveal if user exists for security
                if (user == null)
                    return Result.Success();

                var resetToken = _tokenGenerator.GeneratePasswordResetToken();
                user.GeneratePasswordResetToken(resetToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Send reset email
                var resetLink = $"https://yourapp.com/reset-password?email={email.Value}&token={resetToken}";
                _ = _notificationService.SendPasswordResetAsync(email.Value, user.Username.Value, resetLink, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.ProcessFailed", $"Failed to process request: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Handler for reset password command
    /// </summary>
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token, cancellationToken);
                if (user == null)
                    return Result.Failure(Error.Failure("Error.General", "Invalid or expired reset token"));

                var email = Email.Create(request.Email);
                if (user.Email != email)
                    return Result.Failure(Error.Failure("Error.General", "Invalid reset request"));

                // Validate token (throws if invalid)
                user.ValidatePasswordResetToken(request.Token);

                var newPassword = Password.Create(request.Password);
                var newPasswordHash = _passwordHasher.HashPassword(newPassword);

                user.ChangePassword(newPasswordHash);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return Result.Failure(Error.Failure("Auth.DomainError", ex.Message));
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.ResetPasswordFailed", $"Failed to reset password: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Handler for confirm email command
    /// </summary>
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmEmailCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByEmailConfirmationTokenAsync(request.Token, cancellationToken);
                if (user == null)
                    return Result.Failure(Error.Failure("Error.General", "Invalid confirmation token"));

                var email = Email.Create(request.Email);
                if (user.Email != email)
                    return Result.Failure(Error.Failure("Error.General", "Invalid confirmation request"));

                // ConfirmEmail validates token internally
                user.ConfirmEmail(request.Token);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return Result.Failure(Error.Failure("Auth.DomainError", ex.Message));
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.ConfirmEmailFailed", $"Failed to confirm email: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Handler for resend email confirmation command
    /// </summary>
    public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public ResendEmailConfirmationCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var email = Email.Create(request.Email);
                var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

                if (user == null)
                    return Result.Failure(AuthErrors.UserNotFound);

                if (user.EmailConfirmed)
                    return Result.Failure(Error.Conflict("Auth.EmailAlreadyConfirmed", "Email already confirmed"));

                var confirmationToken = _tokenGenerator.GenerateEmailConfirmationToken();
                user.GenerateEmailConfirmationToken(confirmationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Send confirmation email
                var confirmationLink = $"https://yourapp.com/confirm-email?email={email.Value}&token={confirmationToken}";
                _ = _notificationService.SendEmailConfirmationAsync(email.Value, user.Username.Value, confirmationLink, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    Error.Failure("Auth.ResendConfirmationFailed", $"Failed to resend confirmation: {ex.Message}"));
            }
        }
    }
}

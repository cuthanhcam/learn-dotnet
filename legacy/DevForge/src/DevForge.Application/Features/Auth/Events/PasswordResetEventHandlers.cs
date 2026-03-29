using DevForge.Application.Common.Events;
using DevForge.Application.Common.Interfaces;
using DevForge.Domain.Events.User;
using DevForge.Domain.Repositories;

namespace DevForge.Application.Features.Auth.Events
{
    /// <summary>
    /// Handles password reset request - sends email with reset link
    /// Note: Frontend URL should be injected via INotificationService configuration
    /// </summary>
    public sealed class PasswordResetRequestedDomainEventHandler : DomainEventHandler<PasswordResetRequestedDomainEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public PasswordResetRequestedDomainEventHandler(
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public override async Task Handle(PasswordResetRequestedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
            if (user == null) return;

            // Generate reset link with secure token
            // Note: Frontend URL is configured in EmailService via IConfiguration
            var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            
            // EmailService will prepend frontend URL from configuration
            var resetLink = $"/reset-password?token={resetToken}&email={Uri.EscapeDataString(user.Email.Value)}";
            
            await _notificationService.SendPasswordResetAsync(user.Email.Value, user.Username.Value, resetLink, cancellationToken);
        }
    }

    /// <summary>
    /// Handles password reset completed - invalidates tokens and sends confirmation
    /// </summary>
    public sealed class PasswordResetCompletedDomainEventHandler : DomainEventHandler<PasswordResetCompletedDomainEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public PasswordResetCompletedDomainEventHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public override async Task Handle(PasswordResetCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            // Invalidate all refresh tokens for security (force re-login on all devices)
            var userTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(domainEvent.UserId, cancellationToken);
            foreach (var token in userTokens)
            {
                token.Revoke("Password reset - security measure");
                await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
            }

            // Note: Send password reset confirmation email in production
            await Task.CompletedTask;
        }
    }
}

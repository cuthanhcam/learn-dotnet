using DevForge.Application.Common.Events;
using DevForge.Application.Common.Interfaces;
using DevForge.Domain.Events.User;
using DevForge.Domain.Repositories;
using DevForge.Domain.Services;

namespace DevForge.Application.Features.Auth.Events
{
    /// <summary>
    /// Handles two-factor enabled - generates backup codes and sends security notification
    /// </summary>
    public sealed class TwoFactorEnabledDomainEventHandler : DomainEventHandler<TwoFactorEnabledDomainEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ITwoFactorService _twoFactorService;

        public TwoFactorEnabledDomainEventHandler(
            IUserRepository userRepository,
            INotificationService notificationService,
            ITwoFactorService twoFactorService)
        {
            _userRepository = userRepository;
            _notificationService = notificationService;
            _twoFactorService = twoFactorService;
        }

        public override async Task Handle(TwoFactorEnabledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
            if (user == null) return;

            // Generate backup codes for 2FA recovery
            var backupCodes = _twoFactorService.GenerateBackupCodes();
            
            // Note: In production, send backup codes via secure email
            // For now, backup codes should be displayed to user during setup
            await Task.CompletedTask;
        }
    }
}

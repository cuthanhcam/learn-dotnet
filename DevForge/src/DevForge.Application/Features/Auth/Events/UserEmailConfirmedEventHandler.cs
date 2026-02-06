using DevForge.Application.Common.Events;
using DevForge.Application.Common.Interfaces;
using DevForge.Domain.Events.User;

namespace DevForge.Application.Features.Auth.Events
{
    /// <summary>
    /// Handles user email confirmation - sends welcome email
    /// </summary>
    public sealed class UserEmailConfirmedDomainEventHandler : DomainEventHandler<UserEmailConfirmedDomainEvent>
    {
        private readonly INotificationService _notificationService;

        public UserEmailConfirmedDomainEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override async Task Handle(UserEmailConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            // Send welcome email after confirmation
            await _notificationService.SendWelcomeEmailAsync(
                domainEvent.Email,
                domainEvent.Email.Split('@')[0], // Extract username from email
                cancellationToken);
        }
    }
}

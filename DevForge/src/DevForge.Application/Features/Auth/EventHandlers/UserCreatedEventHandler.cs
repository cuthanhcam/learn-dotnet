using DevForge.Application.Common.Events;
using DevForge.Domain.Events.User;

namespace DevForge.Application.Features.Auth.EventHandlers
{
    /// <summary>
    /// Handles UserCreatedEvent to track user creation for audit trail
    /// </summary>
    public class UserCreatedEventHandler : DomainEventHandler<UserCreatedEvent>
    {
        public override async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            // User creation is logged at the API/Infrastructure layer
            // This handler can be used for additional business logic:
            // - Analytics tracking
            // - Third-party integrations
            // - Audit trail storage
            
            // Note: Welcome email is sent by UserEmailConfirmedEventHandler after email confirmation
            await Task.CompletedTask;
            // - Notify admin
            // - Initialize user preferences
            // - Publish to message queue for other services

            await Task.CompletedTask;
        }
    }
}

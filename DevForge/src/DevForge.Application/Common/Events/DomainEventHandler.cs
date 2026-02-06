using DevForge.Domain.Events;
using MediatR;

namespace DevForge.Application.Common.Events
{
    /// <summary>
    /// Wrapper to convert Domain Events to MediatR Notifications
    /// Used by Infrastructure layer to publish domain events
    /// </summary>
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public TDomainEvent DomainEvent { get; }

        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }

    /// <summary>
    /// Base handler for domain events using MediatR notifications
    /// Handles the wrapper notification from Infrastructure layer
    /// </summary>
    /// <typeparam name="TDomainEvent">Type of domain event</typeparam>
    public abstract class DomainEventHandler<TDomainEvent> : INotificationHandler<DomainEventNotification<TDomainEvent>>
        where TDomainEvent : DomainEvent
    {
        public async Task Handle(DomainEventNotification<TDomainEvent> notification, CancellationToken cancellationToken)
        {
            await Handle(notification.DomainEvent, cancellationToken);
        }

        public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken);
    }
}

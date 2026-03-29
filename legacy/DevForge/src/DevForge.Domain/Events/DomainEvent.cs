namespace DevForge.Domain.Events
{
    /// <summary>
    /// Base class for all domain events
    /// Domain events are dispatched by infrastructure layer (ApplicationDbContext)
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; }
        public Guid EventId { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}

namespace Learning.Persistence.Domain;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(Guid id, DateTimeOffset occurredAt, string type, string payload)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Operation identifier is required.", nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        Id = id;
        OccurredAt = occurredAt;
        Type = type;
        Payload = payload;
    }

    public Guid Id { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset? ProcessedAt { get; private set; }

    public void MarkProcessed(DateTimeOffset processedAt) => ProcessedAt = processedAt;
}

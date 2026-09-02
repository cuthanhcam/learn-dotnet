namespace Learning.Auth.Domain.Documents;

public sealed class LearningDocument
{
    public LearningDocument(Guid id, Guid ownerId, string title)
    {
        if (id == Guid.Empty || ownerId == Guid.Empty)
            throw new ArgumentException("Document and owner identifiers are required.");
        Id = id;
        OwnerId = ownerId;
        Rename(title);
    }

    public Guid Id { get; }
    public Guid OwnerId { get; }
    public string Title { get; private set; } = string.Empty;
    public bool IsPublished { get; private set; }

    public void Rename(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        string normalized = title.Trim();
        if (normalized.Length > 160)
            throw new ArgumentOutOfRangeException(nameof(title), "Title cannot exceed 160 characters.");
        Title = normalized;
    }

    public void Publish() => IsPublished = true;
}

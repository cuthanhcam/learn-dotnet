namespace Learning.Persistence.Domain;

public sealed class Tag
{
    private Tag()
    {
    }

    public Tag(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        string normalized = name.Trim().ToLowerInvariant();
        if (normalized.Length > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Tag name cannot exceed 50 characters.");
        }

        Id = Guid.NewGuid();
        Name = normalized;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ICollection<CourseTag> CourseTags { get; } = new List<CourseTag>();
}

namespace Learning.Persistence.Domain;

public sealed class Category
{
    private Category()
    {
        // EF Core uses this constructor when materializing rows. Application code must use the
        // public constructor so invariants are established before an entity enters the context.
    }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Rename(name);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ICollection<Course> Courses { get; } = new List<Course>();

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        string normalized = name.Trim();
        if (normalized.Length > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Category name cannot exceed 80 characters.");
        }

        Name = normalized;
    }
}

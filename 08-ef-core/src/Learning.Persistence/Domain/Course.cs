namespace Learning.Persistence.Domain;

public sealed class Course
{
    private readonly List<CourseModule> _modules = [];

    private Course()
    {
    }

    public Course(Guid categoryId, string title, string slug, decimal price, DateTimeOffset createdAt)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category identifier is required.", nameof(categoryId));
        }

        Id = Guid.NewGuid();
        CategoryId = categoryId;
        CreatedAt = createdAt;
        UpdateDetails(title, slug, price);
    }

    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long Version { get; private set; } = 1;
    public Category Category { get; private set; } = null!;
    public IReadOnlyCollection<CourseModule> Modules => _modules.AsReadOnly();

    public void UpdateDetails(string title, string slug, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }

        Title = title.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Price = price;
    }

    public CourseModule AddModule(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        var module = new CourseModule(Id, _modules.Count + 1, title.Trim());
        _modules.Add(module);
        return module;
    }

    public void IncrementVersion() => Version = checked(Version + 1);
}

namespace Learning.Persistence.Domain;

public sealed class Course
{
    private readonly List<CourseModule> _modules = [];
    private readonly List<CourseTag> _courseTags = [];

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
    public IReadOnlyCollection<CourseTag> CourseTags => _courseTags.AsReadOnly();

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

    public CourseTag AddTag(Guid tagId)
    {
        if (tagId == Guid.Empty)
        {
            throw new ArgumentException("Tag identifier is required.", nameof(tagId));
        }

        if (_courseTags.Any(link => link.TagId == tagId))
        {
            throw new InvalidOperationException("The course already contains this tag.");
        }

        var link = new CourseTag(Id, tagId);
        _courseTags.Add(link);
        return link;
    }

    public void IncrementVersion() => Version = checked(Version + 1);
}

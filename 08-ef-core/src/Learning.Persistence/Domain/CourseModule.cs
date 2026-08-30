namespace Learning.Persistence.Domain;

public sealed class CourseModule
{
    private CourseModule()
    {
    }

    internal CourseModule(Guid courseId, int order, string title)
    {
        Id = Guid.NewGuid();
        CourseId = courseId;
        Order = order;
        Title = title;
    }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public int Order { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public Course Course { get; private set; } = null!;
}

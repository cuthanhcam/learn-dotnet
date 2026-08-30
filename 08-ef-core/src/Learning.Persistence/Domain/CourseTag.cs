namespace Learning.Persistence.Domain;

/// <summary>
/// An explicit join entity keeps the relational association visible. It can later gain metadata such
/// as AddedAt or AddedBy without replacing an implicit many-to-many mapping across the application.
/// </summary>
public sealed class CourseTag
{
    private CourseTag()
    {
    }

    internal CourseTag(Guid courseId, Guid tagId)
    {
        CourseId = courseId;
        TagId = tagId;
    }

    public Guid CourseId { get; private set; }
    public Guid TagId { get; private set; }
    public Course Course { get; private set; } = null!;
    public Tag Tag { get; private set; } = null!;
}

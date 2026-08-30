namespace Learning.Persistence.Courses;

public sealed record CourseListItem(
    Guid Id,
    string Title,
    string Slug,
    decimal Price,
    string CategoryName,
    int ModuleCount);

public sealed record CourseModuleDetails(Guid Id, int Order, string Title);

public sealed record CourseDetails(
    Guid Id,
    string Title,
    string Slug,
    decimal Price,
    long Version,
    string CategoryName,
    IReadOnlyList<CourseModuleDetails> Modules,
    IReadOnlyList<string> Tags);

public sealed record CoursePage(
    IReadOnlyList<CourseListItem> Items,
    int Page,
    int PageSize,
    int TotalCount);

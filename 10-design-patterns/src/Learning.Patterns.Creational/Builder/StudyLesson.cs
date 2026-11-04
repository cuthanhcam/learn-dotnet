namespace Learning.Patterns.Creational.Builder;

/// <summary>An immutable lesson value. Lab limits count UTF-16 code units.</summary>
public sealed record StudyLesson
{
    public StudyLesson(string title, int minutes)
    {
        Title = PlanInput.Title(title);
        if (minutes is < 1 or > 120)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes));
        }
        Minutes = minutes;
    }

    public string Title { get; }
    public int Minutes { get; }
}

internal static class PlanInput
{
    internal static string Title(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        string normalized = title.Trim();
        if (normalized.Length > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(title));
        }
        return normalized;
    }
}

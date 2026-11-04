namespace Learning.Patterns.Creational.Builder.Refactored;

/// <summary>Shared validation/ownership policy, not a global builder or a public partial product.</summary>
internal sealed class PlanDraft
{
    private string? _title;
    private readonly List<StudyLesson> _lessons = [];
    private int _totalMinutes;

    internal void Begin(string title)
    {
        string normalized = PlanInput.Title(title); // Validate before clearing a previous draft.
        _title = normalized;
        _lessons.Clear();
        _totalMinutes = 0;
    }

    internal void AddLesson(StudyLesson lesson)
    {
        EnsureBegun();
        ArgumentNullException.ThrowIfNull(lesson);
        if (_lessons.Count == StudyPlan.MaximumLessons ||
            _totalMinutes + lesson.Minutes > StudyPlan.MaximumMinutes)
        {
            throw new InvalidOperationException("This lesson would exceed the plan budget.");
        }
        _lessons.Add(lesson);
        _totalMinutes += lesson.Minutes;
    }

    internal StudyPlan Build()
    {
        EnsureBegun();
        if (_lessons.Count == 0)
        {
            throw new InvalidOperationException("Add at least one lesson before building.");
        }
        return new StudyPlan(_title!, _lessons);
    }

    private void EnsureBegun()
    {
        if (_title is null)
        {
            throw new InvalidOperationException("Call Begin before adding lessons or building.");
        }
    }
}

using System.Collections.Immutable;

namespace Learning.Patterns.Creational.Builder;

/// <summary>A complete immutable product. Direct construction remains a supported simpler option.</summary>
public sealed class StudyPlan
{
    public const int MaximumLessons = 12;
    public const int MaximumMinutes = 480;

    public StudyPlan(string title, IEnumerable<StudyLesson> lessons)
    {
        Title = PlanInput.Title(title);
        ArgumentNullException.ThrowIfNull(lessons);
        var snapshot = new List<StudyLesson>();
        int total = 0;
        foreach (StudyLesson lesson in lessons)
        {
            // Inspect at most 13 items, even if the input sequence never ends.
            if (snapshot.Count == MaximumLessons)
            {
                throw new ArgumentException("A plan supports at most 12 lessons.", nameof(lessons));
            }
            ArgumentNullException.ThrowIfNull(lesson);
            total += lesson.Minutes;
            if (total > MaximumMinutes)
            {
                throw new ArgumentException("A plan supports at most 480 minutes.", nameof(lessons));
            }
            snapshot.Add(lesson);
        }
        if (snapshot.Count == 0)
        {
            throw new ArgumentException("A plan needs at least one lesson.", nameof(lessons));
        }
        Lessons = snapshot.ToImmutableArray();
        TotalMinutes = total;
    }

    public string Title { get; }
    public ImmutableArray<StudyLesson> Lessons { get; }
    public int TotalMinutes { get; }
}

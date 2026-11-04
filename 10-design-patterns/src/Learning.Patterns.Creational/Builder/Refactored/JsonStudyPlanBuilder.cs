using System.Text.Json;

namespace Learning.Patterns.Creational.Builder.Refactored;

/// <summary>Same construction steps, a different product representation.</summary>
public sealed class JsonStudyPlanBuilder : IStudyPlanBuilder
{
    private readonly PlanDraft _draft = new();

    public void Begin(string title) => _draft.Begin(title);
    public void AddLesson(StudyLesson lesson) => _draft.AddLesson(lesson);

    public string Build()
    {
        StudyPlan plan = _draft.Build();
        // Use the serializer, not string interpolation, for untrusted title characters.
        return JsonSerializer.Serialize(new
        {
            title = plan.Title,
            totalMinutes = plan.TotalMinutes,
            lessons = plan.Lessons.Select(lesson => new { title = lesson.Title, minutes = lesson.Minutes })
        });
    }
}

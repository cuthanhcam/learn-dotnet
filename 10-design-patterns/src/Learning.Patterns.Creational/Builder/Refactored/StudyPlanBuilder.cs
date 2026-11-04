namespace Learning.Patterns.Creational.Builder.Refactored;

/// <summary>Mutable, single-threaded construction session; Build copies without resetting it.</summary>
public sealed class StudyPlanBuilder : IStudyPlanBuilder
{
    private readonly PlanDraft _draft = new();

    public void Begin(string title) => _draft.Begin(title);
    public void AddLesson(StudyLesson lesson) => _draft.AddLesson(lesson);
    public StudyPlan Build() => _draft.Build();
}

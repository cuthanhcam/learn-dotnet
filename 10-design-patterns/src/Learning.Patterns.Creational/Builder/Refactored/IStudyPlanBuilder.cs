namespace Learning.Patterns.Creational.Builder.Refactored;

/// <summary>Construction steps only; each concrete builder exposes its own product type.</summary>
public interface IStudyPlanBuilder
{
    void Begin(string title);
    void AddLesson(StudyLesson lesson);
}

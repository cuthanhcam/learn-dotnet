namespace Learning.Patterns.Creational.Builder.Refactored;

/// <summary>A reusable construction recipe, independent of the final representation.</summary>
public static class StudyPlanDirector
{
    public static void ConstructFirstDay(IStudyPlanBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Begin("First .NET day");
        builder.AddLesson(new StudyLesson("Read the example", 30));
        builder.AddLesson(new StudyLesson("Run and change it", 45));
        builder.AddLesson(new StudyLesson("Explain the trade-off", 15));
    }
}

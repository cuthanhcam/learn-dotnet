namespace Learning.Patterns.Creational.Builder.Baseline;

/// <summary>Correct for a fixed recipe and one representation; no builder is necessary here.</summary>
public static class DirectPlanFactory
{
    public static StudyPlan FirstDay() => new("First .NET day",
    [
        new StudyLesson("Read the example", 30),
        new StudyLesson("Run and change it", 45),
        new StudyLesson("Explain the trade-off", 15)
    ]);
}

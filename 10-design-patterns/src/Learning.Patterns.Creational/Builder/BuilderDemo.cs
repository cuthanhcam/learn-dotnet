using System.Text.Json;
using Learning.Patterns.Creational.Builder.Baseline;
using Learning.Patterns.Creational.Builder.Refactored;

namespace Learning.Patterns.Creational.Builder;

public static class BuilderDemo
{
    public static void Run(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        StudyPlan baseline = DirectPlanFactory.FirstDay();
        var builder = new StudyPlanBuilder();
        StudyPlanDirector.ConstructFirstDay(builder);
        StudyPlan plan = builder.Build();
        var jsonBuilder = new JsonStudyPlanBuilder();
        StudyPlanDirector.ConstructFirstDay(jsonBuilder);
        using JsonDocument json = JsonDocument.Parse(jsonBuilder.Build());
        output.WriteLine($"Baseline minutes: {baseline.TotalMinutes}");
        output.WriteLine($"Built plan: {plan.Title}; lessons={plan.Lessons.Length}; minutes={plan.TotalMinutes}");
        output.WriteLine($"JSON lessons: {json.RootElement.GetProperty("lessons").GetArrayLength()}");
        builder.Begin("Next day");
        builder.AddLesson(new StudyLesson("Practice", 20));
        output.WriteLine($"Reused builder: {builder.Build().Title}");
        output.WriteLine($"Original snapshot: {plan.Title}; lessons={plan.Lessons.Length}");
    }
}

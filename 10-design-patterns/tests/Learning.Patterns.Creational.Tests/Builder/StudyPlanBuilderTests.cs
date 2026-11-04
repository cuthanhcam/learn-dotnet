using System.Text.Json;
using Learning.Patterns.Creational.Builder;
using Learning.Patterns.Creational.Builder.Baseline;
using Learning.Patterns.Creational.Builder.Refactored;
using Xunit;

namespace Learning.Patterns.Creational.Tests.Builder;

public sealed class StudyPlanBuilderTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Director_BuildsEquivalentRepresentations(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        StudyPlanDirector.ConstructFirstDay(builder);
        StudyPlan actual = Read(builder);
        StudyPlan baseline = DirectPlanFactory.FirstDay();
        Assert.Equal(baseline.Title, actual.Title);
        Assert.Equal(baseline.TotalMinutes, actual.TotalMinutes);
        Assert.Equal(baseline.Lessons.ToArray(), actual.Lessons.ToArray());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BeginIsRequired_AndEmptyDraftCannotBuild(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        Assert.Throws<InvalidOperationException>(() => Read(builder));
        Assert.Throws<InvalidOperationException>(() => builder.AddLesson(new StudyLesson("A", 1)));
        builder.Begin("Plan");
        Assert.Throws<InvalidOperationException>(() => Read(builder));
        builder.AddLesson(new StudyLesson("A", 1));
        Assert.Single(Read(builder).Lessons);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Build_DoesNotResetAndLaterEditsDoNotChangeProducts(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        builder.Begin("Plan");
        builder.AddLesson(new StudyLesson("A", 30));
        StudyPlan first = Read(builder);
        Assert.Equal(first.Lessons.ToArray(), Read(builder).Lessons.ToArray());
        builder.AddLesson(new StudyLesson("B", 20));
        Assert.Single(first.Lessons);
        Assert.Equal(30, first.TotalMinutes);
        Assert.Equal(2, Read(builder).Lessons.Length);
        Assert.Equal(50, Read(builder).TotalMinutes);
        builder.Begin("Next");
        builder.AddLesson(new StudyLesson("C", 10));
        Assert.Equal("Plan", first.Title);
        Assert.Equal("Next", Read(builder).Title);
        Assert.Single(Read(builder).Lessons);
        Assert.Equal(10, Read(builder).TotalMinutes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FailedBegin_PreservesPreviousDraft(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        builder.Begin("Plan");
        builder.AddLesson(new StudyLesson("A", 10));
        Assert.ThrowsAny<ArgumentException>(() => builder.Begin(" "));
        Assert.Equal("Plan", Read(builder).Title);
        Assert.Single(Read(builder).Lessons);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LessonBudget_RejectsBeforeMutation(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        builder.Begin("Plan");
        for (int index = 0; index < 12; index++)
        {
            builder.AddLesson(new StudyLesson("Repeat practice", 1));
        }
        Assert.Throws<InvalidOperationException>(() => builder.AddLesson(new StudyLesson("Extra", 1)));
        Assert.Equal(12, Read(builder).Lessons.Length);
        Assert.Equal(12, Read(builder).TotalMinutes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TimeBudget_RejectsBeforeMutationAndBeginClearsIt(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        builder.Begin("Plan");
        for (int index = 0; index < 4; index++)
        {
            builder.AddLesson(new StudyLesson("Long lesson", 120));
        }
        Assert.Throws<InvalidOperationException>(() => builder.AddLesson(new StudyLesson("Extra", 1)));
        Assert.Equal(480, Read(builder).TotalMinutes);
        Assert.Equal(4, Read(builder).Lessons.Length);
        builder.Begin("Fresh");
        builder.AddLesson(new StudyLesson("New", 120));
        Assert.Equal(120, Read(builder).TotalMinutes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NullLesson_IsRejectedWithoutMutation(bool json)
    {
        IStudyPlanBuilder builder = Create(json);
        builder.Begin("Plan");
        builder.AddLesson(new StudyLesson("Valid", 1));
        Assert.Throws<ArgumentNullException>(() => builder.AddLesson(null!));
        Assert.Single(Read(builder).Lessons);
    }

    [Fact]
    public void DirectConstruction_CopiesCallerCollection()
    {
        var lessons = new List<StudyLesson> { new("Original", 30) };
        var plan = new StudyPlan("Plan", lessons);
        lessons[0] = new StudyLesson("Replaced", 10);
        lessons.Clear();
        Assert.Equal(new StudyLesson("Original", 30), Assert.Single(plan.Lessons));
        Assert.Equal(30, plan.TotalMinutes);
    }

    [Fact]
    public void Json_ProductIsValidAndPreservesEscapedCharacters()
    {
        var builder = new JsonStudyPlanBuilder();
        string title = "Quotes " + (char)34 + " and <tags>";
        string lessonTitle = "Line" + Environment.NewLine + "slash" + (char)92 + " học";
        builder.Begin("  " + title + "  ");
        builder.AddLesson(new StudyLesson(lessonTitle, 10));
        string product = builder.Build();
        using JsonDocument json = JsonDocument.Parse(product);
        Assert.Equal(title, json.RootElement.GetProperty("title").GetString());
        Assert.Equal(lessonTitle, json.RootElement.GetProperty("lessons")[0].GetProperty("title").GetString());
        builder.Begin("Other");
        Assert.Equal(title, json.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public void IndependentBuilders_DoNotShareDrafts()
    {
        var first = new StudyPlanBuilder();
        var second = new StudyPlanBuilder();
        StudyPlanDirector.ConstructFirstDay(first);
        second.Begin("Second");
        second.AddLesson(new StudyLesson("Only second", 1));
        Assert.Equal(3, first.Build().Lessons.Length);
        Assert.Equal(1, second.Build().TotalMinutes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void InvalidTitles_AreRejected(string? title)
    {
        Assert.ThrowsAny<ArgumentException>(() => new StudyLesson(title!, 1));
        Assert.ThrowsAny<ArgumentException>(() => new StudyPlan(title!, [new StudyLesson("A", 1)]));
        Assert.ThrowsAny<ArgumentException>(() => new StudyPlanBuilder().Begin(title!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(121)]
    public void InvalidLessonDuration_IsRejected(int minutes) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new StudyLesson("A", minutes));

    [Fact]
    public void TitleBoundary_IsNormalizedAndBounded()
    {
        string maximum = new('x', 80);
        Assert.Equal(maximum, new StudyLesson("  " + maximum + "  ", 1).Title);
        Assert.Throws<ArgumentOutOfRangeException>(() => new StudyLesson(maximum + "x", 1));
        var builder = new StudyPlanBuilder();
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.Begin(maximum + "x"));
    }

    [Fact]
    public void DirectConstruction_RejectsIncompleteAndOversizedProducts()
    {
        Assert.Throws<ArgumentNullException>(() => new StudyPlan("Plan", null!));
        Assert.Throws<ArgumentException>(() => new StudyPlan("Plan", []));
        Assert.Throws<ArgumentNullException>(() => new StudyPlan("Plan", new StudyLesson[] { null! }));
        Assert.Throws<ArgumentException>(() => new StudyPlan("Plan", Enumerable.Repeat(new StudyLesson("A", 1), 13)));
        Assert.Throws<ArgumentException>(() => new StudyPlan("Plan", Enumerable.Repeat(new StudyLesson("A", 120), 5)));
    }

    [Fact]
    public void DirectConstruction_StopsAfterThirteenthItem()
    {
        int observed = 0;
        IEnumerable<StudyLesson> Infinite()
        {
            while (true)
            {
                observed++;
                if (observed > 13) { throw new Exception("Enumeration exceeded its budget."); }
                yield return new StudyLesson("Repeat", 1);
            }
        }
        Assert.Throws<ArgumentException>(() => new StudyPlan("Plan", Infinite()));
        Assert.Equal(13, observed);
    }

    [Fact]
    public void Director_ReinitializesPreviousDraft()
    {
        var builder = new StudyPlanBuilder();
        builder.Begin("Old");
        builder.AddLesson(new StudyLesson("Old", 120));
        StudyPlanDirector.ConstructFirstDay(builder);
        Assert.Equal("First .NET day", builder.Build().Title);
        Assert.Equal(90, builder.Build().TotalMinutes);
        Assert.Equal(3, builder.Build().Lessons.Length);
        Assert.Throws<ArgumentNullException>(() => StudyPlanDirector.ConstructFirstDay(null!));
    }

    [Fact]
    public void Demo_OutputIsDeterministic()
    {
        Assert.Throws<ArgumentNullException>(() => BuilderDemo.Run(null!));
        using var output = new StringWriter();
        BuilderDemo.Run(output);
        Assert.Equal(string.Join(Environment.NewLine,
            "Baseline minutes: 90", "Built plan: First .NET day; lessons=3; minutes=90",
            "JSON lessons: 3", "Reused builder: Next day",
            "Original snapshot: First .NET day; lessons=3", ""), output.ToString());
    }

    private static IStudyPlanBuilder Create(bool json) =>
        json ? new JsonStudyPlanBuilder() : new StudyPlanBuilder();

    private static StudyPlan Read(IStudyPlanBuilder builder)
    {
        if (builder is StudyPlanBuilder typed) { return typed.Build(); }
        using JsonDocument json = JsonDocument.Parse(((JsonStudyPlanBuilder)builder).Build());
        JsonElement root = json.RootElement;
        StudyLesson[] lessons = root.GetProperty("lessons").EnumerateArray()
            .Select(item => new StudyLesson(item.GetProperty("title").GetString()!, item.GetProperty("minutes").GetInt32()))
            .ToArray();
        var plan = new StudyPlan(root.GetProperty("title").GetString()!, lessons);
        Assert.Equal(plan.TotalMinutes, root.GetProperty("totalMinutes").GetInt32());
        return plan;
    }
}

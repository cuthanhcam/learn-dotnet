using Learning.Patterns.Behavioral.State;
using Learning.Patterns.Behavioral.State.Baseline;
using Learning.Patterns.Behavioral.State.Refactored;
using Xunit;

namespace Learning.Patterns.Behavioral.Tests.State;

public sealed class ArticleWorkflowTests
{
    [Theory]
    [InlineData(ArticleStage.Draft, ArticleAction.Edit, ArticleStage.Draft)]
    [InlineData(ArticleStage.Draft, ArticleAction.Submit, ArticleStage.InReview)]
    [InlineData(ArticleStage.Draft, ArticleAction.Approve, null)]
    [InlineData(ArticleStage.Draft, ArticleAction.Reject, null)]
    [InlineData(ArticleStage.Draft, ArticleAction.Archive, ArticleStage.Archived)]
    [InlineData(ArticleStage.InReview, ArticleAction.Edit, null)]
    [InlineData(ArticleStage.InReview, ArticleAction.Submit, null)]
    [InlineData(ArticleStage.InReview, ArticleAction.Approve, ArticleStage.Published)]
    [InlineData(ArticleStage.InReview, ArticleAction.Reject, ArticleStage.Draft)]
    [InlineData(ArticleStage.InReview, ArticleAction.Archive, null)]
    [InlineData(ArticleStage.Published, ArticleAction.Edit, null)]
    [InlineData(ArticleStage.Published, ArticleAction.Submit, null)]
    [InlineData(ArticleStage.Published, ArticleAction.Approve, null)]
    [InlineData(ArticleStage.Published, ArticleAction.Reject, null)]
    [InlineData(ArticleStage.Published, ArticleAction.Archive, ArticleStage.Archived)]
    [InlineData(ArticleStage.Archived, ArticleAction.Edit, null)]
    [InlineData(ArticleStage.Archived, ArticleAction.Submit, null)]
    [InlineData(ArticleStage.Archived, ArticleAction.Approve, null)]
    [InlineData(ArticleStage.Archived, ArticleAction.Reject, null)]
    [InlineData(ArticleStage.Archived, ArticleAction.Archive, null)]
    public void CompleteTransitionTable_PreservesContractAndBaseline(
        ArticleStage stage, ArticleAction action, ArticleStage? expected)
    {
        var workflow = new ArticleWorkflow("Original");
        var baseline = new SwitchArticle("Original");
        foreach (ArticleAction setup in RouteTo(stage))
        {
            workflow.Apply(setup);
            baseline.Apply(setup);
        }
        ArticleSnapshot before = workflow.Snapshot;
        string? title = action == ArticleAction.Edit ? "Edited" : null;
        if (expected is null)
        {
            Assert.Throws<InvalidOperationException>(() => workflow.Apply(action, title));
            Assert.Throws<InvalidOperationException>(() => baseline.Apply(action, title));
            Assert.Equal(before, workflow.Snapshot);
        }
        else
        {
            workflow.Apply(action, title);
            baseline.Apply(action, title);
            Assert.Equal(expected.Value, workflow.Snapshot.Stage);
            Assert.Equal(action == ArticleAction.Edit ? "Edited" : "Original", workflow.Snapshot.Title);
        }
        Assert.Equal(baseline.Snapshot, workflow.Snapshot);
    }

    [Fact]
    public void Rejection_AllowsRevisionAndResubmission()
    {
        var workflow = new ArticleWorkflow("Original");
        workflow.Apply(ArticleAction.Submit);
        workflow.Apply(ArticleAction.Reject);
        workflow.Apply(ArticleAction.Edit, "Revised");
        workflow.Apply(ArticleAction.Submit);
        workflow.Apply(ArticleAction.Approve);
        Assert.Equal(new ArticleSnapshot(ArticleStage.Published, "Revised"), workflow.Snapshot);
    }

    [Fact]
    public void Snapshots_AreIndependentOfLaterEditsAndCallerCopies()
    {
        var workflow = new ArticleWorkflow("Original");
        ArticleSnapshot saved = workflow.Snapshot;
        ArticleSnapshot forged = saved with { Stage = ArticleStage.Published, Title = "Other" };
        workflow.Apply(ArticleAction.Edit, "Revised");
        Assert.Equal(new ArticleSnapshot(ArticleStage.Draft, "Original"), saved);
        Assert.Equal("Other", forged.Title);
        Assert.Equal(new ArticleSnapshot(ArticleStage.Draft, "Revised"), workflow.Snapshot);
    }

    [Fact]
    public void SeparateArticles_DoNotShareMutableState()
    {
        var first = new ArticleWorkflow("First");
        var second = new ArticleWorkflow("Second");
        first.Apply(ArticleAction.Submit);
        first.Apply(ArticleAction.Approve);
        Assert.Equal(ArticleStage.Draft, second.Snapshot.Stage);
        second.Apply(ArticleAction.Edit, "Changed");
        Assert.Equal(new ArticleSnapshot(ArticleStage.Published, "First"), first.Snapshot);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    public void InvalidTitle_IsRejectedAtConstructionAndEdit(string? title)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ArticleWorkflow(title!));
        Assert.ThrowsAny<ArgumentException>(() => new SwitchArticle(title!));
        var workflow = new ArticleWorkflow("Original");
        Assert.ThrowsAny<ArgumentException>(() => workflow.Apply(ArticleAction.Edit, title));
        Assert.Equal(new ArticleSnapshot(ArticleStage.Draft, "Original"), workflow.Snapshot);
    }

    [Fact]
    public void TitleLimit_IsAppliedAfterTrimmingAndCountsUtf16()
    {
        var workflow = new ArticleWorkflow("  " + new string('x', 80) + "  ");
        Assert.Equal(80, workflow.Snapshot.Title.Length);
        Assert.Throws<ArgumentOutOfRangeException>(() => new ArticleWorkflow(new string('x', 81)));
        Assert.Throws<ArgumentOutOfRangeException>(() => workflow.Apply(ArticleAction.Edit, new string('x', 81)));
        Assert.Equal(80, workflow.Snapshot.Title.Length);
        workflow.Apply(ArticleAction.Edit, "  học .NET  ");
        Assert.Equal("học .NET", workflow.Snapshot.Title);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(99)]
    public void UnknownAction_IsRejectedWithoutMutation(int action)
    {
        var workflow = new ArticleWorkflow("Original");
        var baseline = new SwitchArticle("Original");
        Assert.Throws<ArgumentOutOfRangeException>(() => workflow.Apply((ArticleAction)action));
        Assert.Throws<ArgumentOutOfRangeException>(() => baseline.Apply((ArticleAction)action));
        Assert.Equal(baseline.Snapshot, workflow.Snapshot);
        Assert.Equal(ArticleStage.Draft, workflow.Snapshot.Stage);
    }

    [Theory]
    [InlineData(ArticleAction.Submit)]
    [InlineData(ArticleAction.Approve)]
    [InlineData(ArticleAction.Reject)]
    [InlineData(ArticleAction.Archive)]
    public void NonEditAction_RejectsUnexpectedPayload(ArticleAction action)
    {
        var workflow = new ArticleWorkflow("Original");
        Assert.Throws<ArgumentException>(() => workflow.Apply(action, "Unexpected"));
        Assert.Equal(new ArticleSnapshot(ArticleStage.Draft, "Original"), workflow.Snapshot);
    }

    [Fact]
    public void RepeatedSubmit_IsRejectedAndValidActionStillWorks()
    {
        var workflow = new ArticleWorkflow("Original");
        workflow.Apply(ArticleAction.Submit);
        Assert.Throws<InvalidOperationException>(() => workflow.Apply(ArticleAction.Submit));
        Assert.Equal(ArticleStage.InReview, workflow.Snapshot.Stage);
        workflow.Apply(ArticleAction.Approve);
        Assert.Equal(ArticleStage.Published, workflow.Snapshot.Stage);
    }

    [Fact]
    public void RepeatedDraftEdit_IsAllowedWithoutAdvancingStage()
    {
        var workflow = new ArticleWorkflow("Original");
        workflow.Apply(ArticleAction.Edit, "Same");
        workflow.Apply(ArticleAction.Edit, "Same");
        Assert.Equal(new ArticleSnapshot(ArticleStage.Draft, "Same"), workflow.Snapshot);
    }

    [Fact]
    public void Demo_IsDeterministicAndRejectsMissingWriter()
    {
        Assert.Throws<ArgumentNullException>(() => StateDemo.Run(null!));
        using var output = new StringWriter();
        StateDemo.Run(output);
        Assert.Equal(string.Join(Environment.NewLine,
            "Submit: baseline=InReview; state=InReview",
            "Reject: baseline=Draft; state=Draft",
            "Edit: baseline=Draft; state=Draft",
            "Submit: baseline=InReview; state=InReview",
            "Approve: baseline=Published; state=Published",
            "Archive: baseline=Archived; state=Archived",
            "Final title: Revised State", ""), output.ToString());
    }

    private static ArticleAction[] RouteTo(ArticleStage stage) => stage switch
    {
        ArticleStage.Draft => [],
        ArticleStage.InReview => [ArticleAction.Submit],
        ArticleStage.Published => [ArticleAction.Submit, ArticleAction.Approve],
        ArticleStage.Archived => [ArticleAction.Archive],
        _ => throw new ArgumentOutOfRangeException(nameof(stage))
    };
}

using Learning.Patterns.Behavioral.State.Baseline;
using Learning.Patterns.Behavioral.State.Refactored;

namespace Learning.Patterns.Behavioral.State;

public static class StateDemo
{
    public static void Run(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        var baseline = new SwitchArticle("Learning State");
        var workflow = new ArticleWorkflow("Learning State");
        foreach (ArticleAction action in new[]
        {
            ArticleAction.Submit, ArticleAction.Reject, ArticleAction.Edit,
            ArticleAction.Submit, ArticleAction.Approve, ArticleAction.Archive
        })
        {
            string? title = action == ArticleAction.Edit ? "Revised State" : null;
            baseline.Apply(action, title);
            workflow.Apply(action, title);
            output.WriteLine($"{action}: baseline={baseline.Snapshot.Stage}; state={workflow.Snapshot.Stage}");
        }
        output.WriteLine($"Final title: {workflow.Snapshot.Title}");
    }
}

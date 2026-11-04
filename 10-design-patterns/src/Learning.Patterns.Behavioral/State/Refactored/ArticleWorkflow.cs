namespace Learning.Patterns.Behavioral.State.Refactored;

/// <summary>Context that owns its current state. Use serially on one thread.</summary>
public sealed class ArticleWorkflow
{
    private IArticleState _state = DraftState.Instance;
    private string _title;

    public ArticleWorkflow(string title) => _title = ArticleInput.Title(title);

    public ArticleSnapshot Snapshot => new(_state.Stage, _title);

    public void Apply(ArticleAction action, string? title = null)
    {
        string? normalized = ArticleInput.Validate(action, title);
        // States propose a complete transition before the context commits either field.
        // There are no callbacks, I/O, or user-supplied states between validation and commit.
        ArticleTransition next = _state.Apply(action, _title, normalized);
        _title = next.Title;
        _state = next.State;
    }
}

internal interface IArticleState
{
    ArticleStage Stage { get; }
    ArticleTransition Apply(ArticleAction action, string currentTitle, string? requestedTitle);
}

internal sealed record ArticleTransition(IArticleState State, string Title);

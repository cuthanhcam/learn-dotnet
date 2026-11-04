namespace Learning.Patterns.Behavioral.State.Refactored;

// Shared instances are safe because states retain no article-specific mutable data.
// Keep the catalog internal: callers request actions rather than assign arbitrary states.
internal sealed class DraftState : IArticleState
{
    internal static DraftState Instance { get; } = new();
    private DraftState() { }
    public ArticleStage Stage => ArticleStage.Draft;

    public ArticleTransition Apply(ArticleAction action, string currentTitle, string? requestedTitle) =>
        action switch
        {
            ArticleAction.Edit => new(this, requestedTitle!),
            ArticleAction.Submit => new(ReviewState.Instance, currentTitle),
            ArticleAction.Archive => new(ArchivedState.Instance, currentTitle),
            _ => throw ArticleInput.Illegal(Stage, action)
        };
}

internal sealed class ReviewState : IArticleState
{
    internal static ReviewState Instance { get; } = new();
    private ReviewState() { }
    public ArticleStage Stage => ArticleStage.InReview;

    public ArticleTransition Apply(ArticleAction action, string currentTitle, string? requestedTitle) =>
        action switch
        {
            ArticleAction.Approve => new(PublishedState.Instance, currentTitle),
            ArticleAction.Reject => new(DraftState.Instance, currentTitle),
            _ => throw ArticleInput.Illegal(Stage, action)
        };
}

internal sealed class PublishedState : IArticleState
{
    internal static PublishedState Instance { get; } = new();
    private PublishedState() { }
    public ArticleStage Stage => ArticleStage.Published;

    public ArticleTransition Apply(ArticleAction action, string currentTitle, string? requestedTitle) =>
        action == ArticleAction.Archive
            ? new(ArchivedState.Instance, currentTitle)
            : throw ArticleInput.Illegal(Stage, action);
}

internal sealed class ArchivedState : IArticleState
{
    internal static ArchivedState Instance { get; } = new();
    private ArchivedState() { }
    public ArticleStage Stage => ArticleStage.Archived;

    public ArticleTransition Apply(ArticleAction action, string currentTitle, string? requestedTitle) =>
        throw ArticleInput.Illegal(Stage, action);
}

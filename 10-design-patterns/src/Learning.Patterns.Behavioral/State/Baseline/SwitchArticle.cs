namespace Learning.Patterns.Behavioral.State.Baseline;

/// <summary>A correct small closed workflow; a switch is a reasonable starting point.</summary>
public sealed class SwitchArticle
{
    private ArticleSnapshot _current;

    public SwitchArticle(string title) =>
        _current = new ArticleSnapshot(ArticleStage.Draft, ArticleInput.Title(title));

    public ArticleSnapshot Snapshot => _current;

    public void Apply(ArticleAction action, string? title = null)
    {
        string? normalized = ArticleInput.Validate(action, title);
        ArticleSnapshot next = (_current.Stage, action) switch
        {
            (ArticleStage.Draft, ArticleAction.Edit) => _current with { Title = normalized! },
            (ArticleStage.Draft, ArticleAction.Submit) => _current with { Stage = ArticleStage.InReview },
            (ArticleStage.Draft, ArticleAction.Archive) => _current with { Stage = ArticleStage.Archived },
            (ArticleStage.InReview, ArticleAction.Approve) => _current with { Stage = ArticleStage.Published },
            (ArticleStage.InReview, ArticleAction.Reject) => _current with { Stage = ArticleStage.Draft },
            (ArticleStage.Published, ArticleAction.Archive) => _current with { Stage = ArticleStage.Archived },
            _ => throw ArticleInput.Illegal(_current.Stage, action)
        };
        _current = next;
    }
}

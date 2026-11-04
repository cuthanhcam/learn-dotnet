namespace Learning.Patterns.Behavioral.State;

public enum ArticleStage { Draft, InReview, Published, Archived }

public enum ArticleAction { Edit, Submit, Approve, Reject, Archive }

/// <summary>A read-only view, not a persistence/restoration contract.</summary>
public sealed record ArticleSnapshot(ArticleStage Stage, string Title);

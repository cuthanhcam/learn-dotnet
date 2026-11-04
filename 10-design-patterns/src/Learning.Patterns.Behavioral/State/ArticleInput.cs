namespace Learning.Patterns.Behavioral.State;

internal static class ArticleInput
{
    internal static string Title(string? title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        string normalized = title.Trim();
        if (normalized.Length > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(title), "Title is limited to 80 UTF-16 code units.");
        }
        return normalized;
    }

    internal static string? Validate(ArticleAction action, string? title)
    {
        if (!Enum.IsDefined(action))
        {
            throw new ArgumentOutOfRangeException(nameof(action));
        }
        if (action == ArticleAction.Edit)
        {
            return Title(title);
        }
        if (title is not null)
        {
            throw new ArgumentException("Only Edit accepts a title.", nameof(title));
        }
        return null;
    }

    internal static InvalidOperationException Illegal(ArticleStage stage, ArticleAction action) =>
        new($"Action {action} is not allowed in {stage}.");
}

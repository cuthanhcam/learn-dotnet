using System.Collections.Immutable;

namespace Learning.Patterns.Creational.FactoryMethod;

/// <summary>
/// An immutable input snapshot. Paragraphs are bounded and copied so later caller mutations cannot
/// change the artifact midway through rendering. These are lab budgets, not general document limits.
/// </summary>
public sealed record ReportDocument
{
    public ReportDocument(string title, IEnumerable<string> paragraphs)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(paragraphs);
        if (title.Trim().Length > 100)
            throw new ArgumentOutOfRangeException(nameof(title));
        string[] snapshot = paragraphs.Take(101).ToArray();
        if (snapshot.Length is < 1 or > 100 || snapshot.Any(text => string.IsNullOrWhiteSpace(text) || text.Length > 1000))
            throw new ArgumentException("Provide 1..100 nonblank paragraphs of at most 1000 characters.", nameof(paragraphs));
        Title = title.Trim();
        Paragraphs = snapshot.ToImmutableArray();
    }

    public string Title { get; }
    public ImmutableArray<string> Paragraphs { get; }
}

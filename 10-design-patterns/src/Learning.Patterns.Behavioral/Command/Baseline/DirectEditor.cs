namespace Learning.Patterns.Behavioral.Command.Baseline;

/// <summary>Correct when the caller only needs immediate append with no history or rebinding.</summary>
public sealed class DirectEditor(TextDocument document)
{
    private readonly TextDocument _document = document ?? throw new ArgumentNullException(nameof(document));

    public void Append(string text) => _document.Append(text);
}

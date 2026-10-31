namespace Learning.Patterns.Behavioral.Command;

/// <summary>Captures a receiver and immutable request data; construction does not edit the document.</summary>
public sealed class AppendTextCommand : IEditCommand
{
    private readonly TextDocument _document;
    private readonly string _text;

    public AppendTextCommand(TextDocument document, string text)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrEmpty(text);
        if (text.Length > TextDocument.MaximumLength)
        {
            throw new ArgumentOutOfRangeException(nameof(text));
        }
        _document = document;
        _text = text;
    }

    // Reuse means another execution, not deduplication. Each call produces its own receipt.
    public IUndoEdit Execute() => _document.AppendWithUndo(_text);
}

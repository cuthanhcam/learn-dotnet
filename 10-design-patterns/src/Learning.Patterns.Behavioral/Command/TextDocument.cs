namespace Learning.Patterns.Behavioral.Command;

/// <summary>A bounded, single-threaded receiver. Lengths count UTF-16 code units.</summary>
public sealed class TextDocument
{
    public const int MaximumLength = 4096;
    private DocumentState _state = new("");

    public string Text => _state.Text;

    public void Append(string text) => AppendWithUndo(text);

    internal IUndoEdit AppendWithUndo(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        if (text.Length > MaximumLength - Text.Length)
        {
            throw new InvalidOperationException("The document length limit would be exceeded.");
        }

        DocumentState before = _state;
        DocumentState after = new(before.Text + text);
        var undo = new AppendUndo(this, before, after);
        _state = after;
        return undo;
    }

    // Identity distinguishes equal text reached through unrelated edits. Restoring the previous
    // state identity lets an earlier receipt become valid again after a proper LIFO undo.
    private sealed record DocumentState(string Text);

    private sealed class AppendUndo(
        TextDocument document, DocumentState before, DocumentState after) : IUndoEdit
    {
        private bool _used;

        public void Undo()
        {
            if (_used)
            {
                throw new InvalidOperationException("This edit has already been undone.");
            }
            if (!ReferenceEquals(document._state, after))
            {
                throw new InvalidOperationException("Undo conflicts with a later document edit.");
            }
            document._state = before;
            _used = true;
        }
    }
}

namespace Learning.Patterns.Behavioral.Command;

/// <summary>A single-threaded invoker with bounded undo history and no hidden execution queue.</summary>
public sealed class EditHistory
{
    private readonly Stack<IUndoEdit> _undo;
    private readonly int _capacity;
    private bool _busy;

    public EditHistory(int capacity = 32)
    {
        if (capacity is < 1 or > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _capacity = capacity;
        _undo = new Stack<IUndoEdit>(capacity);
    }

    public int Count => _undo.Count;

    public void Execute(IEditCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        EnsureIdle();
        if (Count == _capacity)
        {
            throw new InvalidOperationException("Undo history is full; undo an edit before executing.");
        }
        _busy = true;
        try
        {
            // Only successful execution enters history. Implementers must honor the atomic
            // contract: this invoker cannot roll back an arbitrary partly failed command.
            IUndoEdit receipt = command.Execute()
                ?? throw new InvalidOperationException("A command must return an undo receipt.");
            _undo.Push(receipt);
        }
        finally
        {
            _busy = false;
        }
    }

    public bool Undo()
    {
        EnsureIdle();
        if (!_undo.TryPeek(out IUndoEdit? receipt))
        {
            return false;
        }
        _busy = true;
        try
        {
            receipt.Undo();
            _undo.Pop(); // Keep a failed/conflicting receipt so the failure is visible.
            return true;
        }
        finally
        {
            _busy = false;
        }
    }

    private void EnsureIdle()
    {
        if (_busy)
        {
            throw new InvalidOperationException("Nested history operations are not supported.");
        }
    }
}

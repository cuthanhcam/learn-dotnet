namespace Learning.Patterns.Behavioral.Command.ModernCSharp;

/// <summary>A callback is sufficient for a local action that needs no undo receipt or history.</summary>
public sealed class DelegateButton(Action action)
{
    private readonly Action _action = action ?? throw new ArgumentNullException(nameof(action));

    public void Click() => _action();
}

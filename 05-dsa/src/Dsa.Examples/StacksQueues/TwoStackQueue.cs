namespace Dsa.Examples.StacksQueues;

public sealed class TwoStackQueue<T>
{
    private readonly Stack<T> _incoming = [];
    private readonly Stack<T> _outgoing = [];

    public int Count => _incoming.Count + _outgoing.Count;

    public void Enqueue(T value)
    {
        _incoming.Push(value);
    }

    public T Dequeue()
    {
        MoveIncomingIfNeeded();
        return _outgoing.Pop();
    }

    public T Peek()
    {
        MoveIncomingIfNeeded();
        return _outgoing.Peek();
    }

    private void MoveIncomingIfNeeded()
    {
        if (_outgoing.Count > 0)
        {
            return;
        }

        while (_incoming.Count > 0)
        {
            _outgoing.Push(_incoming.Pop());
        }
    }
}

namespace AsyncConcurrency.Examples.Synchronization;

public sealed class ThreadSafeCounter
{
    private int _value;

    public int Value => Volatile.Read(ref _value);

    public int Increment()
    {
        // ++ is a read-modify-write sequence. Interlocked makes the complete
        // transition atomic and also provides the required memory ordering.
        return Interlocked.Increment(ref _value);
    }
}

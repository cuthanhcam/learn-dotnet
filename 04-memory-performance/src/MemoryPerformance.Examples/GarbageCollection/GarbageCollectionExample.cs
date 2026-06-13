namespace MemoryPerformance.Examples.GarbageCollection;

/// <summary>
/// Shows how allocation pressure, generations, finalization, and deterministic cleanup differ.
/// </summary>
public static class GarbageCollectionExample
{
    public static void Run()
    {
        GcSnapshot before = CaptureSnapshot();
        long allocated = AllocateShortLivedObjects(iterations: 1_000, payloadSize: 128);
        GcSnapshot after = CaptureSnapshot();

        Console.WriteLine($"Allocated bytes on current thread: {allocated:N0}");
        Console.WriteLine($"Collections before: {before}");
        Console.WriteLine($"Collections after:  {after}");
        Console.WriteLine($"Disposable demo: {UseDisposableResource()}");
        Console.WriteLine($"Large object candidate length: {CreateLargeBuffer().Length:N0}");
    }

    public static GcSnapshot CaptureSnapshot()
    {
        return new GcSnapshot(
            Generation0Collections: GC.CollectionCount(0),
            Generation1Collections: GC.CollectionCount(1),
            Generation2Collections: GC.CollectionCount(2),
            TotalMemoryBytes: GC.GetTotalMemory(forceFullCollection: false));
    }

    public static long AllocateShortLivedObjects(int iterations, int payloadSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(iterations);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(payloadSize);

        long before = GC.GetAllocatedBytesForCurrentThread();

        for (int i = 0; i < iterations; i++)
        {
            byte[] payload = new byte[payloadSize];
            payload[0] = (byte)(i % 255);
        }

        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    public static string UseDisposableResource()
    {
        var buffer = new DisposableBuffer(capacity: 16);
        int sum;

        using (buffer)
        {
            buffer.Write(42);
            buffer.Write(100);
            sum = buffer.Sum;
        }

        return $"sum={sum}; disposed={buffer.IsDisposed}";
    }

    public static byte[] CreateLargeBuffer()
    {
        return new byte[100_000];
    }
}

public readonly record struct GcSnapshot(
    int Generation0Collections,
    int Generation1Collections,
    int Generation2Collections,
    long TotalMemoryBytes)
{
    public override string ToString()
    {
        return $"Gen0={Generation0Collections}, Gen1={Generation1Collections}, Gen2={Generation2Collections}, Heap={TotalMemoryBytes:N0}";
    }
}

public sealed class DisposableBuffer : IDisposable
{
    private int[]? _values;
    private int _position;

    public DisposableBuffer(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _values = new int[capacity];
    }

    public bool IsDisposed => _values is null;

    public int Sum
    {
        get
        {
            ObjectDisposedException.ThrowIf(_values is null, this);
            return _values.Take(_position).Sum();
        }
    }

    public void Write(int value)
    {
        ObjectDisposedException.ThrowIf(_values is null, this);

        if (_position >= _values.Length)
        {
            throw new InvalidOperationException("Buffer is full.");
        }

        _values[_position++] = value;
    }

    public void Dispose()
    {
        _values = null;
        _position = 0;
    }
}

using System.Buffers;

namespace MemoryPerformance.Examples.SpanMemoryPooling;

/// <summary>
/// Owns one array rented from an <see cref="ArrayPool{T}"/> and exposes only the requested length.
/// </summary>
/// <remarks>
/// This educational owner makes the rent/return lifecycle explicit. It is a class so copies share
/// one disposal state; a copyable struct owner could accidentally return the same array twice.
/// </remarks>
public sealed class PooledBuffer<T> : IMemoryOwner<T>
{
    private readonly ArrayPool<T> _pool;
    private readonly bool _clearOnReturn;
    private T[]? _array;

    public PooledBuffer(
        int length,
        bool clearOnReturn = false,
        ArrayPool<T>? pool = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        _pool = pool ?? ArrayPool<T>.Shared;
        _clearOnReturn = clearOnReturn;
        _array = _pool.Rent(length);
        Length = length;
    }

    public int Length { get; }

    public Memory<T> Memory
    {
        get
        {
            T[] array = _array ?? throw new ObjectDisposedException(nameof(PooledBuffer<T>));

            // Rent can return a larger size class. Restricting Memory to Length prevents callers
            // from accidentally treating unrequested, potentially stale elements as owned data.
            return array.AsMemory(0, Length);
        }
    }

    public Span<T> Span => Memory.Span;

    public void Dispose()
    {
        // Exchange transfers the array out of this owner exactly once. Concurrent or repeated
        // Dispose calls therefore cannot return the same instance to the pool twice.
        T[]? array = Interlocked.Exchange(ref _array, null);
        if (array is null)
        {
            return;
        }

        _pool.Return(array, clearArray: _clearOnReturn);
    }
}

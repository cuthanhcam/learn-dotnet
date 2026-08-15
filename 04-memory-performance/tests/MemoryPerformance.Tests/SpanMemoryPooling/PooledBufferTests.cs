using System.Buffers;
using MemoryPerformance.Examples.SpanMemoryPooling;

namespace MemoryPerformance.Tests.SpanMemoryPooling;

public sealed class PooledBufferTests
{
    [Fact]
    public void Memory_ExposesRequestedLengthRatherThanPhysicalArrayLength()
    {
        var pool = new TrackingArrayPool<int>(physicalLength: 16);
        using var owner = new PooledBuffer<int>(length: 5, pool: pool);

        Assert.Equal(5, owner.Memory.Length);
        Assert.Equal(5, owner.Span.Length);
    }

    [Fact]
    public void Dispose_ReturnsArrayExactlyOnceWithClearPolicy()
    {
        var pool = new TrackingArrayPool<string>(physicalLength: 8);
        var owner = new PooledBuffer<string>(
            length: 3,
            clearOnReturn: true,
            pool: pool);
        owner.Span[0] = "sensitive";

        owner.Dispose();
        owner.Dispose();

        Assert.Equal(1, pool.ReturnCount);
        Assert.True(pool.LastClearArray);
    }

    [Fact]
    public void Memory_AfterDisposeThrowsObjectDisposedException()
    {
        var owner = new PooledBuffer<byte>(length: 4);
        owner.Dispose();

        Assert.Throws<ObjectDisposedException>(() => owner.Memory);
    }

    private sealed class TrackingArrayPool<T>(int physicalLength) : ArrayPool<T>
    {
        private readonly T[] _array = new T[physicalLength];

        public int ReturnCount { get; private set; }
        public bool LastClearArray { get; private set; }

        public override T[] Rent(int minimumLength)
        {
            if (minimumLength > _array.Length)
            {
                throw new InvalidOperationException("The test pool is too small for this request.");
            }

            return _array;
        }

        public override void Return(T[] array, bool clearArray = false)
        {
            Assert.Same(_array, array);
            ReturnCount++;
            LastClearArray = clearArray;

            if (clearArray)
            {
                Array.Clear(array);
            }
        }
    }
}

using MemoryPerformance.Examples.GarbageCollection;

namespace MemoryPerformance.Tests.GarbageCollection;

public class GarbageCollectionExampleTests
{
    [Fact]
    public void AllocateShortLivedObjects_Returns_Positive_Allocation_Delta()
    {
        long allocated = GarbageCollectionExample.AllocateShortLivedObjects(10, 64);

        Assert.True(allocated > 0);
    }

    [Fact]
    public void DisposableBuffer_Throws_After_Dispose()
    {
        var buffer = new DisposableBuffer(2);
        buffer.Write(10);

        buffer.Dispose();

        Assert.True(buffer.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => buffer.Write(20));
    }

    [Fact]
    public void UseDisposableResource_Disposes_Buffer_After_Block()
    {
        string result = GarbageCollectionExample.UseDisposableResource();

        Assert.Equal("sum=142; disposed=True", result);
    }

    [Fact]
    public void CreateLargeBuffer_Returns_Large_Object_Candidate()
    {
        byte[] buffer = GarbageCollectionExample.CreateLargeBuffer();

        Assert.True(buffer.Length >= 85_000);
    }
}

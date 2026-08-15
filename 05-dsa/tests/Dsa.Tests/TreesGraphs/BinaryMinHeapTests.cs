using Dsa.Examples.TreesGraphs;

namespace Dsa.Tests.TreesGraphs;

public sealed class BinaryMinHeapTests
{
    [Fact]
    public void HeapifyAndRemoveMin_ReturnValuesInSortedOrder()
    {
        var heap = new BinaryMinHeap<int>([7, 2, 9, 1, 5, 1]);
        var removed = new List<int>();

        while (heap.Count > 0)
        {
            removed.Add(heap.RemoveMin());
        }

        Assert.Equal([1, 1, 2, 5, 7, 9], removed);
    }

    [Fact]
    public void Add_RestoresHeapInvariant()
    {
        var heap = new BinaryMinHeap<string>(StringComparer.OrdinalIgnoreCase);

        heap.Add("Zulu");
        heap.Add("alpha");
        heap.Add("Bravo");

        Assert.Equal("alpha", heap.Peek());
    }

    [Fact]
    public void EmptyHeap_RejectsPeekAndRemoval()
    {
        var heap = new BinaryMinHeap<int>();

        Assert.Throws<InvalidOperationException>(() => heap.Peek());
        Assert.Throws<InvalidOperationException>(() => heap.RemoveMin());
    }
}

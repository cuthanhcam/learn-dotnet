using Dsa.Examples.LinkedLists;

namespace Dsa.Tests.LinkedLists;

public sealed class AdvancedLinkedListTests
{
    [Fact]
    public void FindCycleEntry_ReturnsTheFirstNodeInCycle()
    {
        var first = new ListNode<int>(1);
        var entry = new ListNode<int>(2);
        var third = new ListNode<int>(3);
        var fourth = new ListNode<int>(4);
        first.Next = entry;
        entry.Next = third;
        third.Next = fourth;
        fourth.Next = entry;

        ListNode<int>? result = LinkedListAlgorithms.FindCycleEntry(first);

        Assert.Same(entry, result);
        Assert.Equal(3, LinkedListAlgorithms.GetCycleLength(first));
    }

    [Fact]
    public void FindCycleEntry_AcyclicListReturnsNullAndZeroLength()
    {
        ListNode<int>? head = LinkedListsExample.FromValues(1, 2, 3);

        Assert.Null(LinkedListAlgorithms.FindCycleEntry(head));
        Assert.Equal(0, LinkedListAlgorithms.GetCycleLength(head));
    }

    [Fact]
    public void FindIntersection_UsesNodeIdentityRatherThanEqualValues()
    {
        var shared = new ListNode<int>(7) { Next = new ListNode<int>(8) };
        var first = new ListNode<int>(1) { Next = new ListNode<int>(7) { Next = shared } };
        var second = new ListNode<int>(2) { Next = shared };

        Assert.Same(shared, LinkedListAlgorithms.FindIntersection(first, second));
    }

    [Theory]
    [InlineData(1, new[] { 1, 2, 3 })]
    [InlineData(2, new[] { 1, 2, 4 })]
    [InlineData(4, new[] { 2, 3, 4 })]
    public void RemoveNthFromEnd_HandlesHeadMiddleAndTail(int position, int[] expected)
    {
        ListNode<int>? result = LinkedListAlgorithms.RemoveNthFromEnd(
            LinkedListsExample.FromValues(1, 2, 3, 4),
            position);

        Assert.Equal(expected, LinkedListsExample.ToArray(result));
    }
}

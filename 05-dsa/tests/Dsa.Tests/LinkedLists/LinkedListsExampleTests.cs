using Dsa.Examples.LinkedLists;

namespace Dsa.Tests.LinkedLists;

public sealed class LinkedListsExampleTests
{
    [Fact]
    public void ReverseRewiresNodes()
    {
        ListNode<int>? head = LinkedListsExample.FromValues(1, 2, 3, 4);

        ListNode<int>? reversed = LinkedListsExample.Reverse(head);

        Assert.Equal([4, 3, 2, 1], LinkedListsExample.ToArray(reversed));
    }

    [Fact]
    public void MergeSortedRelinksInSortedOrder()
    {
        ListNode<int>? merged = LinkedListsExample.MergeSorted(
            LinkedListsExample.FromValues(1, 4, 7),
            LinkedListsExample.FromValues(2, 3, 8));

        Assert.Equal([1, 2, 3, 4, 7, 8], LinkedListsExample.ToArray(merged));
    }

    [Theory]
    [InlineData(new[] { 1 }, 1)]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 3)]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6 }, 4)]
    public void FindMiddleReturnsSecondMiddleForEvenLength(int[] values, int expected)
    {
        Assert.Equal(expected, LinkedListsExample.FindMiddle(LinkedListsExample.FromValues(values))?.Value);
    }

    [Fact]
    public void HasCycleUsesReferenceEquality()
    {
        ListNode<int> first = new(1);
        ListNode<int> second = new(2);
        ListNode<int> third = new(3);
        first.Next = second;
        second.Next = third;
        third.Next = second;

        Assert.True(LinkedListsExample.HasCycle(first));
    }
}

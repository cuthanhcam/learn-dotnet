using Dsa.Examples.StacksQueues;

namespace Dsa.Tests.StacksQueues;

public sealed class StacksQueuesExampleTests
{
    [Theory]
    [InlineData("({[]})", true)]
    [InlineData("(]", false)]
    [InlineData("(()", false)]
    [InlineData("plain text", true)]
    public void IsValidParenthesesMatchesNestedPairs(string input, bool expected)
    {
        Assert.Equal(expected, StacksQueuesExample.IsValidParentheses(input));
    }

    [Fact]
    public void NextGreaterElementsUsesMonotonicStack()
    {
        Assert.Equal([4, 2, 4, -1, -1], StacksQueuesExample.NextGreaterElements([2, 1, 2, 4, 3]));
    }

    [Fact]
    public void TwoStackQueueKeepsFifoOrder()
    {
        TwoStackQueue<int> queue = new();

        queue.Enqueue(10);
        queue.Enqueue(20);
        Assert.Equal(10, queue.Dequeue());
        queue.Enqueue(30);

        Assert.Equal(20, queue.Peek());
        Assert.Equal(20, queue.Dequeue());
        Assert.Equal(30, queue.Dequeue());
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void BreadthFirstLevelsVisitsByDistance()
    {
        Dictionary<string, string[]> graph = new()
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = ["D"],
            ["D"] = []
        };

        Assert.Equal(["0:A", "1:B", "1:C", "2:D"], StacksQueuesExample.BreadthFirstLevels(graph, "A"));
    }
}

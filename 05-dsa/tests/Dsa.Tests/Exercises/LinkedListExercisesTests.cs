using Dsa.Exercises;

namespace Dsa.Tests.Exercises;

public sealed class LinkedListExercisesTests
{
    [Fact]
    public void RemoveElementsRemovesMatchingNodes()
    {
        LinkedListExercises.Node head = Build(1, 2, 6, 3, 6);

        LinkedListExercises.Node? result = LinkedListExercises.RemoveElements(head, 6);

        Assert.Equal([1, 2, 3], ToArray(result));
    }

    [Fact]
    public void KthFromEndReturnsRequestedValue()
    {
        Assert.Equal(4, LinkedListExercises.KthFromEnd(Build(1, 2, 3, 4, 5), 2));
    }

    private static LinkedListExercises.Node Build(params int[] values)
    {
        LinkedListExercises.Node head = new(values[0]);
        LinkedListExercises.Node tail = head;

        foreach (int value in values.Skip(1))
        {
            tail.Next = new LinkedListExercises.Node(value);
            tail = tail.Next;
        }

        return head;
    }

    private static int[] ToArray(LinkedListExercises.Node? head)
    {
        List<int> values = [];

        while (head is not null)
        {
            values.Add(head.Value);
            head = head.Next;
        }

        return values.ToArray();
    }
}

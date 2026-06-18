namespace Dsa.Exercises;

public static class LinkedListExercises
{
    public sealed class Node(int value)
    {
        public int Value { get; set; } = value;

        public Node? Next { get; set; }
    }

    public static Node? RemoveElements(Node? head, int value)
    {
        Node sentinel = new(0) { Next = head };
        Node current = sentinel;

        while (current.Next is not null)
        {
            if (current.Next.Value == value)
            {
                current.Next = current.Next.Next;
            }
            else
            {
                current = current.Next;
            }
        }

        return sentinel.Next;
    }

    public static int KthFromEnd(Node head, int k)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k);

        Node? fast = head;
        Node? slow = head;

        for (int i = 0; i < k; i++)
        {
            if (fast is null)
            {
                throw new ArgumentException("k cannot exceed list length.");
            }

            fast = fast.Next;
        }

        while (fast is not null)
        {
            fast = fast.Next;
            slow = slow!.Next;
        }

        return slow!.Value;
    }
}

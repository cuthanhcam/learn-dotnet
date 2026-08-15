namespace Dsa.Examples.LinkedLists;

public static class LinkedListAlgorithms
{
    public static ListNode<T>? FindCycleEntry<T>(ListNode<T>? head)
    {
        ListNode<T>? slow = head;
        ListNode<T>? fast = head;

        do
        {
            if (fast?.Next is null)
            {
                return null;
            }

            slow = slow!.Next;
            fast = fast.Next.Next;
        }
        while (!ReferenceEquals(slow, fast));

        // If the non-cyclic prefix has length μ and the cycle has length λ,
        // the meeting point and head are equally far from the cycle entry
        // modulo λ. Moving both one step therefore finds the entry.
        slow = head;
        while (!ReferenceEquals(slow, fast))
        {
            slow = slow!.Next;
            fast = fast!.Next;
        }

        return slow;
    }

    public static int GetCycleLength<T>(ListNode<T>? head)
    {
        ListNode<T>? entry = FindCycleEntry(head);
        if (entry is null)
        {
            return 0;
        }

        int length = 1;
        for (ListNode<T>? current = entry.Next; !ReferenceEquals(current, entry); current = current!.Next)
        {
            length++;
        }

        return length;
    }

    public static ListNode<T>? FindIntersection<T>(ListNode<T>? first, ListNode<T>? second)
    {
        // This variant intentionally requires acyclic lists. With shared cycles,
        // “first intersection” needs an additional definition and algorithm.
        if (FindCycleEntry(first) is not null || FindCycleEntry(second) is not null)
        {
            throw new ArgumentException("Intersection requires acyclic lists.");
        }

        int firstLength = GetLength(first);
        int secondLength = GetLength(second);

        while (firstLength > secondLength)
        {
            first = first!.Next;
            firstLength--;
        }

        while (secondLength > firstLength)
        {
            second = second!.Next;
            secondLength--;
        }

        while (first is not null && second is not null)
        {
            if (ReferenceEquals(first, second))
            {
                return first;
            }

            first = first.Next;
            second = second.Next;
        }

        return null;
    }

    public static ListNode<T>? RemoveNthFromEnd<T>(ListNode<T>? head, int positionFromEnd)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(positionFromEnd);

        // A sentinel makes deletion of the original head follow the same pointer
        // update as every other deletion.
        var sentinel = new ListNode<T>(default!) { Next = head };
        ListNode<T> lead = sentinel;
        ListNode<T> follow = sentinel;

        for (int step = 0; step < positionFromEnd; step++)
        {
            lead = lead.Next ?? throw new ArgumentOutOfRangeException(
                nameof(positionFromEnd),
                "Position exceeds the list length.");
        }

        while (lead.Next is not null)
        {
            lead = lead.Next;
            follow = follow.Next!;
        }

        follow.Next = follow.Next!.Next;
        return sentinel.Next;
    }

    private static int GetLength<T>(ListNode<T>? head)
    {
        int length = 0;
        for (ListNode<T>? current = head; current is not null; current = current.Next)
        {
            length++;
        }

        return length;
    }
}

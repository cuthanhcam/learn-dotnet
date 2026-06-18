namespace Dsa.Examples.LinkedLists;

public static class LinkedListsExample
{
    public static ListNode<int>? FromValues(params int[] values)
    {
        ListNode<int>? head = null;
        ListNode<int>? tail = null;

        foreach (int value in values)
        {
            ListNode<int> node = new(value);

            if (head is null)
            {
                head = node;
                tail = node;
            }
            else
            {
                tail!.Next = node;
                tail = node;
            }
        }

        return head;
    }

    public static int[] ToArray(ListNode<int>? head, int maxNodes = 10_000)
    {
        List<int> values = [];
        ListNode<int>? current = head;
        int visited = 0;

        while (current is not null && visited < maxNodes)
        {
            values.Add(current.Value);
            current = current.Next;
            visited++;
        }

        return values.ToArray();
    }

    public static ListNode<int>? Reverse(ListNode<int>? head)
    {
        ListNode<int>? previous = null;
        ListNode<int>? current = head;

        while (current is not null)
        {
            ListNode<int>? next = current.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        return previous;
    }

    public static ListNode<int>? MergeSorted(ListNode<int>? left, ListNode<int>? right)
    {
        ListNode<int> sentinel = new(0);
        ListNode<int> tail = sentinel;

        while (left is not null && right is not null)
        {
            if (left.Value <= right.Value)
            {
                tail.Next = left;
                left = left.Next;
            }
            else
            {
                tail.Next = right;
                right = right.Next;
            }

            tail = tail.Next;
        }

        tail.Next = left ?? right;
        return sentinel.Next;
    }

    public static ListNode<int>? FindMiddle(ListNode<int>? head)
    {
        ListNode<int>? slow = head;
        ListNode<int>? fast = head;

        while (fast?.Next is not null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        return slow;
    }

    public static bool HasCycle(ListNode<int>? head)
    {
        ListNode<int>? slow = head;
        ListNode<int>? fast = head;

        while (fast?.Next is not null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;

            if (ReferenceEquals(slow, fast))
            {
                return true;
            }
        }

        return false;
    }

    public static void Run()
    {
        ListNode<int>? list = FromValues(1, 2, 3, 4);
        ListNode<int>? reversed = Reverse(list);
        ListNode<int>? merged = MergeSorted(FromValues(1, 4, 7), FromValues(2, 3, 8));

        Console.WriteLine("Linked lists");
        Console.WriteLine($"Reversed: {string.Join(", ", ToArray(reversed))}");
        Console.WriteLine($"Merged sorted: {string.Join(", ", ToArray(merged))}");
        Console.WriteLine($"Middle of 10,20,30,40,50: {FindMiddle(FromValues(10, 20, 30, 40, 50))?.Value}");
    }
}

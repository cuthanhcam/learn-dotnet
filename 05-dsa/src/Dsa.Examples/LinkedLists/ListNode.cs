namespace Dsa.Examples.LinkedLists;

public sealed class ListNode<T>(T value)
{
    public T Value { get; set; } = value;

    public ListNode<T>? Next { get; set; }
}

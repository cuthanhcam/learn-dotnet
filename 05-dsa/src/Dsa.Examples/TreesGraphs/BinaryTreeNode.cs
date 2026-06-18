namespace Dsa.Examples.TreesGraphs;

public sealed class BinaryTreeNode<T>(T value)
{
    public T Value { get; set; } = value;

    public BinaryTreeNode<T>? Left { get; set; }

    public BinaryTreeNode<T>? Right { get; set; }
}

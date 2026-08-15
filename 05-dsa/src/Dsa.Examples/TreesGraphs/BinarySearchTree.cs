namespace Dsa.Examples.TreesGraphs;

public sealed class BinarySearchTree<T>(IComparer<T>? comparer = null)
{
    private readonly IComparer<T> _comparer = comparer ?? Comparer<T>.Default;

    public BinaryTreeNode<T>? Root { get; private set; }
    public int Count { get; private set; }

    public bool Add(T value)
    {
        if (Root is null)
        {
            Root = new BinaryTreeNode<T>(value);
            Count = 1;
            return true;
        }

        BinaryTreeNode<T> current = Root;
        while (true)
        {
            int comparison = _comparer.Compare(value, current.Value);
            if (comparison == 0)
            {
                return false;
            }

            if (comparison < 0)
            {
                if (current.Left is null)
                {
                    current.Left = new BinaryTreeNode<T>(value);
                    Count++;
                    return true;
                }

                current = current.Left;
            }
            else
            {
                if (current.Right is null)
                {
                    current.Right = new BinaryTreeNode<T>(value);
                    Count++;
                    return true;
                }

                current = current.Right;
            }
        }
    }

    public bool Contains(T value)
    {
        BinaryTreeNode<T>? current = Root;
        while (current is not null)
        {
            int comparison = _comparer.Compare(value, current.Value);
            if (comparison == 0)
            {
                return true;
            }

            current = comparison < 0 ? current.Left : current.Right;
        }

        return false;
    }

    public bool Remove(T value)
    {
        (Root, bool removed) = Remove(Root, value);
        if (removed)
        {
            Count--;
        }

        return removed;
    }

    public T[] InOrder()
    {
        var values = new List<T>(Count);
        var stack = new Stack<BinaryTreeNode<T>>();
        BinaryTreeNode<T>? current = Root;

        while (current is not null || stack.Count > 0)
        {
            while (current is not null)
            {
                stack.Push(current);
                current = current.Left;
            }

            current = stack.Pop();
            values.Add(current.Value);
            current = current.Right;
        }

        return values.ToArray();
    }

    private (BinaryTreeNode<T>? Node, bool Removed) Remove(BinaryTreeNode<T>? node, T value)
    {
        if (node is null)
        {
            return (null, false);
        }

        int comparison = _comparer.Compare(value, node.Value);
        if (comparison < 0)
        {
            (node.Left, bool removed) = Remove(node.Left, value);
            return (node, removed);
        }

        if (comparison > 0)
        {
            (node.Right, bool removed) = Remove(node.Right, value);
            return (node, removed);
        }

        if (node.Left is null)
        {
            return (node.Right, true);
        }

        if (node.Right is null)
        {
            return (node.Left, true);
        }

        // For two children, copy the in-order successor and then remove that
        // successor from the right subtree. Count is decremented only once by
        // the public method.
        BinaryTreeNode<T> successor = FindMinimum(node.Right);
        node.Value = successor.Value;
        (node.Right, _) = Remove(node.Right, successor.Value);
        return (node, true);
    }

    private static BinaryTreeNode<T> FindMinimum(BinaryTreeNode<T> node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node;
    }
}

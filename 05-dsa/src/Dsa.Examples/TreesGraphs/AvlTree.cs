namespace Dsa.Examples.TreesGraphs;

/// <summary>
/// An ordered set backed by an AVL self-balancing binary search tree.
/// Duplicate values, as defined by the comparer, are not inserted.
/// </summary>
public sealed class AvlTree<T>(IComparer<T>? comparer = null)
{
    private readonly IComparer<T> _comparer = comparer ?? Comparer<T>.Default;
    private Node? _root;

    public int Count { get; private set; }
    public int Height => GetHeight(_root);

    public bool Add(T value)
    {
        (_root, bool added) = Insert(_root, value);
        if (added)
        {
            Count++;
        }

        return added;
    }

    public bool Contains(T value)
    {
        Node? current = _root;
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
        (_root, bool removed) = Delete(_root, value);
        if (removed)
        {
            Count--;
        }

        return removed;
    }

    public T[] InOrder()
    {
        var result = new List<T>(Count);
        var stack = new Stack<Node>();
        Node? current = _root;

        while (current is not null || stack.Count > 0)
        {
            while (current is not null)
            {
                stack.Push(current);
                current = current.Left;
            }

            current = stack.Pop();
            result.Add(current.Value);
            current = current.Right;
        }

        return result.ToArray();
    }

    private (Node Node, bool Added) Insert(Node? node, T value)
    {
        if (node is null)
        {
            return (new Node(value), true);
        }

        int comparison = _comparer.Compare(value, node.Value);
        if (comparison == 0)
        {
            return (node, false);
        }

        bool added;
        if (comparison < 0)
        {
            (node.Left, added) = Insert(node.Left, value);
        }
        else
        {
            (node.Right, added) = Insert(node.Right, value);
        }

        // A duplicate changes neither structure nor height, so no rotation is required.
        return (added ? Rebalance(node) : node, added);
    }

    private (Node? Node, bool Removed) Delete(Node? node, T value)
    {
        if (node is null)
        {
            return (null, false);
        }

        int comparison = _comparer.Compare(value, node.Value);
        bool removed;
        if (comparison < 0)
        {
            (node.Left, removed) = Delete(node.Left, value);
        }
        else if (comparison > 0)
        {
            (node.Right, removed) = Delete(node.Right, value);
        }
        else
        {
            removed = true;
            if (node.Left is null)
            {
                return (node.Right, true);
            }

            if (node.Right is null)
            {
                return (node.Left, true);
            }

            // Replace a two-child node with its in-order successor. The recursive deletion
            // below removes that successor without changing the public Count a second time.
            Node successor = FindMinimum(node.Right);
            node.Value = successor.Value;
            (node.Right, _) = Delete(node.Right, successor.Value);
        }

        return (removed ? Rebalance(node) : node, removed);
    }

    private static Node Rebalance(Node node)
    {
        UpdateHeight(node);
        int balance = Balance(node);

        if (balance > 1)
        {
            // A left-right shape first rotates the child toward a left-left shape.
            if (Balance(node.Left!) < 0)
            {
                node.Left = RotateLeft(node.Left!);
            }

            return RotateRight(node);
        }

        if (balance < -1)
        {
            // A right-left shape first rotates the child toward a right-right shape.
            if (Balance(node.Right!) > 0)
            {
                node.Right = RotateRight(node.Right!);
            }

            return RotateLeft(node);
        }

        return node;
    }

    private static Node RotateRight(Node root)
    {
        Node pivot = root.Left!;
        Node? movedSubtree = pivot.Right;
        pivot.Right = root;
        root.Left = movedSubtree;

        // Update the demoted root before the promoted pivot because pivot height depends on it.
        UpdateHeight(root);
        UpdateHeight(pivot);
        return pivot;
    }

    private static Node RotateLeft(Node root)
    {
        Node pivot = root.Right!;
        Node? movedSubtree = pivot.Left;
        pivot.Left = root;
        root.Right = movedSubtree;

        UpdateHeight(root);
        UpdateHeight(pivot);
        return pivot;
    }

    private static Node FindMinimum(Node node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node;
    }

    private static int Balance(Node node) => GetHeight(node.Left) - GetHeight(node.Right);

    private static int GetHeight(Node? node) => node?.Height ?? 0;

    private static void UpdateHeight(Node node) =>
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

    private sealed class Node(T value)
    {
        public T Value { get; set; } = value;
        public Node? Left { get; set; }
        public Node? Right { get; set; }
        public int Height { get; set; } = 1;
    }
}

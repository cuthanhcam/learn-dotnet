namespace Dsa.Examples.TreesGraphs;

public sealed class DisjointSet
{
    private readonly int[] _parent;
    private readonly int[] _size;

    public DisjointSet(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        _parent = Enumerable.Range(0, count).ToArray();
        _size = Enumerable.Repeat(1, count).ToArray();
        ComponentCount = count;
    }

    public int Count => _parent.Length;
    public int ComponentCount { get; private set; }

    public int Find(int item)
    {
        ValidateItem(item);

        int root = item;
        while (root != _parent[root])
        {
            root = _parent[root];
        }

        // Path compression makes every node on the discovered path point
        // directly to the root, accelerating later operations.
        while (item != root)
        {
            int next = _parent[item];
            _parent[item] = root;
            item = next;
        }

        return root;
    }

    public bool Union(int first, int second)
    {
        int firstRoot = Find(first);
        int secondRoot = Find(second);
        if (firstRoot == secondRoot)
        {
            return false;
        }

        // Attach the smaller tree below the larger root to limit height.
        if (_size[firstRoot] < _size[secondRoot])
        {
            (firstRoot, secondRoot) = (secondRoot, firstRoot);
        }

        _parent[secondRoot] = firstRoot;
        _size[firstRoot] += _size[secondRoot];
        ComponentCount--;
        return true;
    }

    public bool Connected(int first, int second) => Find(first) == Find(second);

    private void ValidateItem(int item)
    {
        if ((uint)item >= (uint)_parent.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}

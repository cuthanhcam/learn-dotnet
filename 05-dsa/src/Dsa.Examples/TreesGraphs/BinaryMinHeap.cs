namespace Dsa.Examples.TreesGraphs;

/// <summary>
/// A binary min-heap backed by a contiguous array-like list.
/// The smallest value is always at index zero.
/// </summary>
public sealed class BinaryMinHeap<T>
{
    private readonly List<T> _items;
    private readonly IComparer<T> _comparer;

    public BinaryMinHeap(IComparer<T>? comparer = null)
    {
        _items = [];
        _comparer = comparer ?? Comparer<T>.Default;
    }

    public BinaryMinHeap(IEnumerable<T> values, IComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        _items = values.ToList();
        _comparer = comparer ?? Comparer<T>.Default;

        // Every leaf is already a valid one-node heap. Sifting internal nodes downward in
        // reverse level order builds the complete heap in O(n), not O(n log n).
        for (int index = ParentIndex(_items.Count - 1); index >= 0; index--)
        {
            SiftDown(index);
        }
    }

    public int Count => _items.Count;

    public void Add(T value)
    {
        _items.Add(value);
        SiftUp(_items.Count - 1);
    }

    public T Peek()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("The heap is empty.");
        }

        return _items[0];
    }

    public T RemoveMin()
    {
        T minimum = Peek();
        int lastIndex = _items.Count - 1;

        // Move the last leaf to the root, shrink the logical tree, then restore the
        // parent <= children invariant along one downward path.
        _items[0] = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        if (_items.Count > 0)
        {
            SiftDown(0);
        }

        return minimum;
    }

    private void SiftUp(int index)
    {
        while (index > 0)
        {
            int parent = ParentIndex(index);
            if (_comparer.Compare(_items[parent], _items[index]) <= 0)
            {
                return;
            }

            (_items[parent], _items[index]) = (_items[index], _items[parent]);
            index = parent;
        }
    }

    private void SiftDown(int index)
    {
        while (true)
        {
            int left = checked((index * 2) + 1);
            if (left >= _items.Count)
            {
                return;
            }

            int right = left + 1;
            int smallerChild = right < _items.Count &&
                _comparer.Compare(_items[right], _items[left]) < 0
                    ? right
                    : left;

            if (_comparer.Compare(_items[index], _items[smallerChild]) <= 0)
            {
                return;
            }

            (_items[index], _items[smallerChild]) = (_items[smallerChild], _items[index]);
            index = smallerChild;
        }
    }

    private static int ParentIndex(int index) => (index - 1) / 2;
}

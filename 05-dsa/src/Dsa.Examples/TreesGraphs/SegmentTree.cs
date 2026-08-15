namespace Dsa.Examples.TreesGraphs;

/// <summary>
/// A segment tree for point assignment and range-sum queries.
/// Public ranges use the half-open convention [startInclusive, endExclusive).
/// </summary>
public sealed class SegmentTree
{
    private readonly long[] _tree;
    private readonly int _leafCapacity;

    public SegmentTree(IEnumerable<long> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        long[] source = values.ToArray();
        Count = source.Length;

        // Round leaf storage up to a power of two. Unused leaves contain zero, the
        // identity element for addition, so they do not change parent aggregates.
        _leafCapacity = 1;
        while (_leafCapacity < Math.Max(1, Count))
        {
            _leafCapacity = checked(_leafCapacity * 2);
        }

        _tree = new long[checked(_leafCapacity * 2)];
        Array.Copy(source, 0, _tree, _leafCapacity, source.Length);

        for (int node = _leafCapacity - 1; node > 0; node--)
        {
            _tree[node] = checked(_tree[node * 2] + _tree[(node * 2) + 1]);
        }
    }

    public int Count { get; }

    public void Update(int index, long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

        int node = _leafCapacity + index;
        _tree[node] = value;

        // Only ancestors of the changed leaf can have a different aggregate.
        while ((node /= 2) > 0)
        {
            _tree[node] = checked(_tree[node * 2] + _tree[(node * 2) + 1]);
        }
    }

    public long Query(int startInclusive, int endExclusive)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startInclusive);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endExclusive, Count);
        if (startInclusive > endExclusive)
        {
            throw new ArgumentException("The start of a range cannot follow its end.");
        }

        int left = _leafCapacity + startInclusive;
        int right = _leafCapacity + endExclusive;
        long sum = 0;

        while (left < right)
        {
            // A right child has no sibling fully contained on its left, so include it now.
            if ((left & 1) == 1)
            {
                sum = checked(sum + _tree[left++]);
            }

            // Convert the exclusive right boundary to the fully contained left sibling.
            if ((right & 1) == 1)
            {
                sum = checked(sum + _tree[--right]);
            }

            left /= 2;
            right /= 2;
        }

        return sum;
    }
}

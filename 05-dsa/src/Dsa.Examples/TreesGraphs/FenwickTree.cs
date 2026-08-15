namespace Dsa.Examples.TreesGraphs;

public sealed class FenwickTree
{
    private readonly long[] _tree;
    private readonly long[] _values;

    public FenwickTree(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        _tree = new long[length + 1];
        _values = new long[length];
    }

    public FenwickTree(IEnumerable<long> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = values.ToArray();
        _tree = new long[_values.Length + 1];

        for (int index = 0; index < _values.Length; index++)
        {
            AddInternal(index, _values[index]);
        }
    }

    public int Length => _values.Length;

    public long this[int index]
    {
        get
        {
            ValidateIndex(index);
            return _values[index];
        }
        set
        {
            ValidateIndex(index);
            long delta = checked(value - _values[index]);
            _values[index] = value;
            AddInternal(index, delta);
        }
    }

    public long PrefixSum(int inclusiveEnd)
    {
        ValidateIndex(inclusiveEnd);

        long sum = 0;
        // Internally the tree is one-based. Clearing the least-significant set
        // bit moves to the parent range responsible for the preceding prefix.
        for (int position = inclusiveEnd + 1; position > 0; position -= position & -position)
        {
            sum = checked(sum + _tree[position]);
        }

        return sum;
    }

    public long RangeSum(int start, int inclusiveEnd)
    {
        ValidateIndex(start);
        ValidateIndex(inclusiveEnd);
        if (start > inclusiveEnd)
        {
            throw new ArgumentException("Start must not exceed the inclusive end.", nameof(start));
        }

        return start == 0
            ? PrefixSum(inclusiveEnd)
            : checked(PrefixSum(inclusiveEnd) - PrefixSum(start - 1));
    }

    private void AddInternal(int zeroBasedIndex, long delta)
    {
        for (int position = zeroBasedIndex + 1; position < _tree.Length; position += position & -position)
        {
            _tree[position] = checked(_tree[position] + delta);
        }
    }

    private void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)_values.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}

namespace Dsa.Examples.DynamicProgramming;

public readonly record struct Interval
{
    public Interval(int start, int end)
    {
        if (end < start)
        {
            throw new ArgumentException("Interval end must not precede its start.");
        }

        Start = start;
        End = end;
    }

    public int Start { get; }
    public int End { get; }
}

public static class GreedyAlgorithms
{
    public static Interval[] SelectMaximumNonOverlapping(IEnumerable<Interval> intervals)
    {
        ArgumentNullException.ThrowIfNull(intervals);

        Interval[] ordered = intervals
            .OrderBy(static interval => interval.End)
            .ThenBy(static interval => interval.Start)
            .ToArray();

        var selected = new List<Interval>();
        int? previousEnd = null;
        foreach (Interval interval in ordered)
        {
            if (previousEnd is null || interval.Start >= previousEnd)
            {
                selected.Add(interval);
                previousEnd = interval.End;
            }
        }

        return selected.ToArray();
    }
}

using CoreDotNet.Exercises;

namespace CoreDotNet.Tests.Exercises;

public sealed class CoreDotNetExercisesTests
{
    [Fact]
    public void WordFrequency_CountsCaseInsensitively()
    {
        IReadOnlyDictionary<string, int> counts = WordFrequency.Count("LINQ, linq; Events");

        Assert.Equal(2, counts["linq"]);
        Assert.Equal(1, counts["events"]);
    }

    [Fact]
    public void QueryExercises_ReturnsMaterializedOrderedDistinctResult()
    {
        var source = new List<int> { 4, 2, 2, 3 };

        int[] result = QueryExercises.GetDistinctEvenSquares(source);
        source.Add(6);

        Assert.Equal([4, 16], result);
    }

    [Fact]
    public void QueryExercises_ReportsCheckedOverflow()
    {
        Assert.Throws<OverflowException>(
            () => QueryExercises.GetDistinctEvenSquares([int.MaxValue - 1]));
    }

    [Fact]
    public void Result_SeparatesSuccessAndFailureContracts()
    {
        Result<int> success = Result<int>.Success(42);
        Result<int> failure = Result<int>.Failure("Value was unavailable.");

        Assert.True(success.IsSuccess);
        Assert.Equal(42, success.Value);
        Assert.Null(success.Error);

        Assert.False(failure.IsSuccess);
        Assert.Equal(default, failure.Value);
        Assert.Equal("Value was unavailable.", failure.Error);
    }

    [Fact]
    public void ThresholdCounter_RaisesEventOnceWhenBoundaryIsCrossed()
    {
        var counter = new ThresholdCounter(2);
        var observedValues = new List<int>();
        counter.ThresholdReached += (_, eventArgs) => observedValues.Add(eventArgs.Value);

        counter.Increment();
        counter.Increment();
        counter.Increment();

        Assert.Equal([2], observedValues);
        Assert.Equal(3, counter.Value);
    }

    [Fact]
    public void ThresholdCounter_UnsubscribedHandlerIsNotRetainedByBehavior()
    {
        var counter = new ThresholdCounter(1);
        int calls = 0;
        EventHandler<ThresholdReachedEventArgs> handler = (_, _) => calls++;
        counter.ThresholdReached += handler;
        counter.ThresholdReached -= handler;

        counter.Increment();

        Assert.Equal(0, calls);
    }
}

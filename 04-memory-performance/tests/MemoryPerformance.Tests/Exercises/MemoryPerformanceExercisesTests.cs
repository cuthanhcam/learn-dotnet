using MemoryPerformance.Exercises;

namespace MemoryPerformance.Tests.Exercises;

public sealed class MemoryPerformanceExercisesTests
{
    [Fact]
    public void UniqueWords_NormalizesDeduplicatesAndOrders()
    {
        IReadOnlyList<string> words = AllocationExercises.UniqueWords("span  GC span allocation");

        Assert.Equal(["ALLOCATION", "GC", "SPAN"], words);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "0")]
    [InlineData(4, "0,1,2,3")]
    public void JoinNumbersEfficiently_ReturnsExpectedSequence(int count, string expected)
    {
        Assert.Equal(expected, AllocationExercises.JoinNumbersEfficiently(count));
    }

    [Fact]
    public void ParseThreeNumbers_ParsesExactlyThreeIntegers()
    {
        int[] values = SpanExercises.ParseThreeNumbers("10,-2,30");

        Assert.Equal([10, -2, 30], values);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1,2")]
    [InlineData("1,2,3,4")]
    [InlineData("1,two,3")]
    public void ParseThreeNumbers_RejectsInvalidShape(string input)
    {
        Assert.Throws<FormatException>(() => SpanExercises.ParseThreeNumbers(input));
    }

    [Fact]
    public void RemoveWhitespace_HandlesInputLargerThanStackThreshold()
    {
        string input = string.Concat(Enumerable.Repeat("a ", 100));

        string result = SpanExercises.RemoveWhitespace(input);

        Assert.Equal(new string('a', 100), result);
    }

    [Theory]
    [InlineData(" customer-id ", "CUSTOMER_ID")]
    [InlineData("two words", "TWOWORDS")]
    [InlineData("", "")]
    public void NormalizeKey_AppliesDocumentedNormalization(string input, string expected)
    {
        Assert.Equal(expected, PooledBufferExercises.NormalizeKey(input));
    }
}

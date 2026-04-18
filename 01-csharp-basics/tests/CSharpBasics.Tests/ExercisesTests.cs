using CSharpBasics.Exercises.Easy;
using CSharpBasics.Exercises.Hard;
using CSharpBasics.Exercises.Medium;

namespace CSharpBasics.Tests;

public class ExercisesTests
{
    [Fact]
    public void EasyExercises_WorkAsExpected()
    {
        Assert.Equal(10, SumNumbers.CalculateSum(1, 2, 3, 4));
        Assert.Equal(10, MaxOfThree.GetMax(3, 10, 7));
        Assert.Equal(new List<int> { 2, 3, 4, 5 }, SimpleLoop.GenerateRange(2, 5));
        Assert.Equal("Int32", VariableTypes.GetTypeName(123));
        Assert.Equal("null", VariableTypes.GetTypeName(null));
        Assert.True(EvenOdd.IsEven(42));
        Assert.Equal("Hello, Alex!", MethodBasics.Greet("Alex"));
        Assert.Equal("Hello, world!", MethodBasics.Greet(""));
        Assert.Equal(77.0, TemperatureConverter.CelsiusToFahrenheit(25), 1);
    }

    [Fact]
    public void MediumExercises_WorkAsExpected()
    {
        Assert.Equal("olleh", ReverseString.Reverse("hello"));
        Assert.True(Palindrome.IsPalindrome("radar"));
        Assert.False(Palindrome.IsPalindrome("hello"));
        Assert.Equal("Anonymous", NullDisplay.GetDisplayName(null));
        Assert.Equal("Ann", NullDisplay.GetDisplayName("  Ann  "));

        var counts = CountWords.Count("C# is fun, c# is fast");
        Assert.Equal(2, counts["c#"]);
        Assert.Equal(2, counts["is"]);
        Assert.Equal(1, counts["fun"]);
        Assert.Equal(1, counts["fast"]);

        Assert.Equal(new List<int> { 0, 1, 1, 2, 3, 5, 8 }, FibonacciSequence.Generate(7));
        Assert.Equal(new List<int> { 2, 3, 5, 7 }, PrimeNumbers.GetPrimesUpTo(10));
        Assert.Equal(new List<int> { 1, 2, 3 }, RemoveDuplicates.GetDistinctValues(new[] { 1, 1, 2, 3, 2 }));
    }

    [Fact]
    public void HardExercises_WorkAsExpected()
    {
        Assert.Equal(5d, BasicCalculator.Evaluate(10, 2, '/'));
        Assert.Equal(12d, BasicCalculator.Evaluate(10, 2, '+'));
        Assert.Throws<DivideByZeroException>(() => BasicCalculator.Evaluate(10, 0, '/'));

        var groups = new Dictionary<string, List<int>>
        {
            ["A"] = new List<int> { 3, 1 },
            ["B"] = new List<int> { 2, 3 },
            ["C"] = new List<int> { 9 }
        };

        Assert.Equal(new List<int> { 1, 2, 3, 9 }, NestedCollections.FlattenDistinctSorted(groups));
        Assert.Equal("Ann=10; Ben=N/A", StudentReport.BuildReport(new Dictionary<string, int?>
        {
            ["Ann"] = 10,
            ["Ben"] = null
        }));
        Assert.Equal((10, 4), MemoryBucket.Analyze());
    }
}

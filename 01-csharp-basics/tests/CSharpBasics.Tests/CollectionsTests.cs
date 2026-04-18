using CSharpBasics.Examples.Collections;
using Xunit;

namespace CSharpBasics.Tests;

public class CollectionsTests
{
    [Fact]
    public void BubbleSort_WithRandomArray_ReturnsSorted()
    {
        int[] array = [64, 34, 25];
        ArraysExample.BubbleSort(array);
        Assert.Equal([25, 34, 64], array);
    }

    [Fact]
    public void ArraysExample_CoreOperations_WorkCorrectly()
    {
        Assert.Equal(["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"], ArraysExample.CreateWeekdays());

        int[][] jagged = [[1, 2], [], [3], null!];
        Assert.Equal([1, 2, 3], ArraysExample.Flatten(jagged));

        var matrix = ArraysExample.CreateMatrix(2, 2);
        Assert.Equal(1, matrix[0, 0]);
        Assert.Equal(4, matrix[1, 1]);

        int[] values = [5, 10, 15];
        Assert.Equal(30, ArraysExample.Sum(values));
        Assert.Equal(10, ArraysExample.Average(values));
        Assert.Equal(15, ArraysExample.FindMax(values));
        Assert.Equal(5, ArraysExample.FindMin(values));
        Assert.Equal(1, ArraysExample.LinearSearch(values, 10));
        Assert.Equal(-1, ArraysExample.LinearSearch(values, 100));
    }

    [Fact]
    public void ArraysExample_SearchAndMutationHelpers_WorkCorrectly()
    {
        int[] sorted = [1, 3, 5, 7, 9];
        Assert.Equal(3, ArraysExample.BinarySearchSorted(sorted, 7));
        Assert.Equal(-1, ArraysExample.BinarySearchSorted(sorted, 8));
        Assert.Throws<ArgumentException>(() => ArraysExample.BinarySearchSorted([3, 1, 2], 1));

        Assert.True(ArraysExample.TryGetAt(sorted, 0, out int first));
        Assert.Equal(1, first);
        Assert.False(ArraysExample.TryGetAt(sorted, 99, out int _));

        var copy = ArraysExample.ManualCopy(sorted);
        copy[0] = 999;
        Assert.Equal(1, sorted[0]);

        ArraysExample.ReverseInPlace(sorted);
        Assert.Equal([9, 7, 5, 3, 1], sorted);
        Assert.True(ArraysExample.IsSortedAscending([1, 2, 2, 3]));
        Assert.False(ArraysExample.IsSortedAscending([2, 1]));

        Assert.Equal(2, ArraysExample.CountOccurrences([1, 2, 1, 3], 1));
        Assert.Equal([3, 1, 2], ArraysExample.DistinctPreserveOrder([3, 1, 3, 2, 1]));

        int[] rotate = [1, 2, 3, 4, 5];
        ArraysExample.RotateRight(rotate, 2);
        Assert.Equal([4, 5, 1, 2, 3], rotate);
    }

    [Fact]
    public void ListExample_Operations_WorkCorrectly()
    {
        Assert.Equal(["Red", "Green", "Blue", "Yellow"], ListExample.CreateColorList());
        Assert.Equal([18, 30], ListExample.FilterAdults([12, 18, 30]));

        var numbers = new List<int> { 1, -2, 3, -4 };
        ListExample.RemoveNegativeNumbers(numbers);
        Assert.Equal([1, 3], numbers);

        Assert.Equal(2.0, ListExample.CalculateAverage([1, 2, 3]));
        Assert.Equal(0.0, ListExample.CalculateAverage(Array.Empty<int>()));

        ListExample.InsertAtStart(numbers, 99);
        Assert.Equal(99, numbers[0]);
        Assert.Equal([99, 1], ListExample.TakeFirst(numbers, 2));
        Assert.Empty(ListExample.TakeFirst(numbers, 0));

        var grouped = ListExample.GroupByCategory([-2, 0, 5, 50, 200]);
        Assert.Equal(1, grouped["Negative"]);
        Assert.Equal(1, grouped["Zero"]);

        Assert.True(ListExample.TryGetAt(numbers, 1, out int value));
        Assert.Equal(1, value);
        Assert.False(ListExample.TryGetAt(numbers, 100, out int _));

        Assert.Equal(2, ListExample.BinarySearchSorted([1, 3, 5, 7], 5));
        Assert.Throws<ArgumentException>(() => ListExample.BinarySearchSorted([5, 1, 3], 1));
        Assert.True(ListExample.IsSortedAscending([1, 2, 3]));
        Assert.False(ListExample.IsSortedAscending([3, 2, 1]));
    }

    [Fact]
    public void DictionaryExample_Operations_WorkCorrectly()
    {
        var wc = DictionaryExample.BuildWordCount("hello Hello world");
        Assert.Equal(2, wc["hello"]);
        Assert.Equal(1, wc["world"]);

        Assert.Equal("Hanoi", DictionaryExample.TryGetCapital("vn"));
        Assert.Null(DictionaryExample.TryGetCapital("xx"));

        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        DictionaryExample.IncreaseValue(dict, "api", 2);
        DictionaryExample.IncreaseValue(dict, "API", 3);
        Assert.Equal(5, dict["api"]);

        DictionaryExample.MergeCounts(dict, new Dictionary<string, int> { ["api"] = 1, ["db"] = 4 });
        Assert.Equal(6, dict["api"]);
        Assert.Equal(4, dict["db"]);

        var grouped = DictionaryExample.GroupByFirstLetter(["apple", "avocado", "banana", " "]);
        Assert.Equal(2, grouped['A'].Count);
        Assert.Single(grouped['B']);

        var charCounts = DictionaryExample.CountCharacters("AaA", ignoreCase: true);
        Assert.Equal(3, charCounts['A']);

        var most = DictionaryExample.FindMostFrequent(new Dictionary<string, int> { ["a"] = 1, ["b"] = 3 });
        Assert.NotNull(most);
        Assert.Equal("b", most!.Value.Item);
        Assert.Null(DictionaryExample.FindMostFrequent(new Dictionary<string, int>()));
    }

    [Fact]
    public void HashSetExample_Operations_WorkCorrectly()
    {
        Assert.Equal([1, 2, 3], HashSetExample.RemoveDuplicates([1, 1, 2, 3]).OrderBy(x => x));
        var intersection = HashSetExample.IntersectTags(["API", "dotnet"], ["api", "js"]);
        Assert.Single(intersection);
        Assert.Contains("api", intersection, StringComparer.OrdinalIgnoreCase);
        Assert.Equal(["api", "dotnet", "sql"], HashSetExample.UnionTags(["api"], ["dotnet", "sql"]).OrderBy(x => x));
        Assert.Equal(["api"], HashSetExample.DifferenceTags(["api", "dotnet"], ["dotnet"]).OrderBy(x => x));
        Assert.Equal(["a", "d"], HashSetExample.SymmetricDifferenceTags(["a", "b"], ["b", "d"]).OrderBy(x => x));
        Assert.True(HashSetExample.IsSubset(["a"], ["a", "b"]));
        Assert.True(HashSetExample.IsSuperset(["a", "b"], ["a"]));
        Assert.True(HashSetExample.HasAnyCommon(["a", "b"], ["x", "b"]));

        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "api" };
        Assert.False(HashSetExample.TryAddUnique(set, "API"));
        Assert.True(HashSetExample.TryAddUnique(set, "cloud"));
    }

    [Fact]
    public void EnumerableExample_Operations_WorkCorrectly()
    {
        Assert.Equal([2, 4], EnumerableExample.FilterEvenNumbers([1, 2, 3, 4]).ToArray());
        Assert.Equal(["A", "B"], EnumerableExample.ToUpperWords(["a", null, " ", "b"]).ToArray());
        Assert.Equal(6, EnumerableExample.Sum([1, 2, 3]));
        Assert.Equal([1, 2], EnumerableExample.Take([1, 2, 3], 2).ToArray());
        Assert.Equal([3, 4], EnumerableExample.Skip([1, 2, 3, 4], 2).ToArray());
        Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableExample.Skip([1, 2], -1).ToArray());

        var batches = EnumerableExample.Batch([1, 2, 3, 4, 5], 2).ToArray();
        Assert.Equal(3, batches.Length);
        Assert.Equal([1, 2], batches[0]);
        Assert.Equal([5], batches[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => EnumerableExample.Batch([1], 0).ToArray());

        Assert.Equal([5, 6, 7], EnumerableExample.CountFrom(5).Take(3).ToArray());
        Assert.Equal([0L, 1L, 1L, 2L, 3L, 5L], EnumerableExample.GenerateFibonacci().Take(6).ToArray());
    }
}

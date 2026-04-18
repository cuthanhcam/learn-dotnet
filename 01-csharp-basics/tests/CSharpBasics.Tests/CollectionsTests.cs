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
}

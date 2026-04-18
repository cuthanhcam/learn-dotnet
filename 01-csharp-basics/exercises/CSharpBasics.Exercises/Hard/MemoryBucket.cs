namespace CSharpBasics.Exercises.Hard;

public static class MemoryBucket
{
    // Memory exercise for reference and value behavior.
    public static (int ValueCopy, int ReferenceCount) Analyze()
    {
        int value = 10;
        int valueCopy = value;

        var numbers = new List<int> { 1, 2, 3 };
        var sameReference = numbers;
        sameReference.Add(4);

        return (valueCopy, numbers.Count);
    }
}

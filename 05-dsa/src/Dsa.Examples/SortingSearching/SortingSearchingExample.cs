namespace Dsa.Examples.SortingSearching;

public static class SortingSearchingExample
{
    public static int[] MergeSort(ReadOnlySpan<int> values)
    {
        if (values.Length <= 1)
        {
            return values.ToArray();
        }

        int middle = values.Length / 2;
        int[] left = MergeSort(values[..middle]);
        int[] right = MergeSort(values[middle..]);

        return Merge(left, right);
    }

    public static int BinarySearch(ReadOnlySpan<int> sortedValues, int target)
    {
        int left = 0;
        int right = sortedValues.Length - 1;

        while (left <= right)
        {
            int middle = left + ((right - left) / 2);

            if (sortedValues[middle] == target)
            {
                return middle;
            }

            if (sortedValues[middle] < target)
            {
                left = middle + 1;
            }
            else
            {
                right = middle - 1;
            }
        }

        return -1;
    }

    public static int LowerBound(ReadOnlySpan<int> sortedValues, int target)
    {
        int left = 0;
        int right = sortedValues.Length;

        while (left < right)
        {
            int middle = left + ((right - left) / 2);

            if (sortedValues[middle] < target)
            {
                left = middle + 1;
            }
            else
            {
                right = middle;
            }
        }

        return left;
    }

    public static int FirstBadVersion(int versionCount, Predicate<int> isBadVersion)
    {
        int left = 1;
        int right = versionCount;

        while (left < right)
        {
            int middle = left + ((right - left) / 2);

            if (isBadVersion(middle))
            {
                right = middle;
            }
            else
            {
                left = middle + 1;
            }
        }

        return left;
    }

    public static int QuickSelectKthSmallest(int[] values, int k)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(k, values.Length);

        int targetIndex = k - 1;
        int left = 0;
        int right = values.Length - 1;

        while (left <= right)
        {
            int pivotIndex = Partition(values, left, right);

            if (pivotIndex == targetIndex)
            {
                return values[pivotIndex];
            }

            if (pivotIndex < targetIndex)
            {
                left = pivotIndex + 1;
            }
            else
            {
                right = pivotIndex - 1;
            }
        }

        throw new InvalidOperationException("Selection failed.");
    }

    public static void Run()
    {
        int[] values = [5, 2, 9, 1, 5, 6];

        Console.WriteLine("Sorting and searching");
        Console.WriteLine($"Merge sort: {string.Join(", ", MergeSort(values))}");
        Console.WriteLine($"Binary search for 5: {BinarySearch([1, 2, 5, 5, 6, 9], 5)}");
        Console.WriteLine($"Lower bound for 5: {LowerBound([1, 2, 5, 5, 6, 9], 5)}");
        Console.WriteLine($"3rd smallest: {QuickSelectKthSmallest([5, 2, 9, 1, 5, 6], 3)}");
    }

    private static int[] Merge(int[] left, int[] right)
    {
        int[] result = new int[left.Length + right.Length];
        int leftIndex = 0;
        int rightIndex = 0;
        int writeIndex = 0;

        while (leftIndex < left.Length && rightIndex < right.Length)
        {
            if (left[leftIndex] <= right[rightIndex])
            {
                result[writeIndex++] = left[leftIndex++];
            }
            else
            {
                result[writeIndex++] = right[rightIndex++];
            }
        }

        while (leftIndex < left.Length)
        {
            result[writeIndex++] = left[leftIndex++];
        }

        while (rightIndex < right.Length)
        {
            result[writeIndex++] = right[rightIndex++];
        }

        return result;
    }

    private static int Partition(int[] values, int left, int right)
    {
        int pivot = values[right];
        int storeIndex = left;

        for (int i = left; i < right; i++)
        {
            if (values[i] <= pivot)
            {
                (values[storeIndex], values[i]) = (values[i], values[storeIndex]);
                storeIndex++;
            }
        }

        (values[storeIndex], values[right]) = (values[right], values[storeIndex]);
        return storeIndex;
    }
}

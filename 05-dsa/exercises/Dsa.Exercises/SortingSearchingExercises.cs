namespace Dsa.Exercises;

public static class SortingSearchingExercises
{
    public static int SearchInsertPosition(ReadOnlySpan<int> sortedValues, int target)
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

    public static int[] SortSquares(ReadOnlySpan<int> sortedValues)
    {
        int[] result = new int[sortedValues.Length];
        int left = 0;
        int right = sortedValues.Length - 1;
        int write = result.Length - 1;

        while (left <= right)
        {
            int leftSquare = sortedValues[left] * sortedValues[left];
            int rightSquare = sortedValues[right] * sortedValues[right];

            if (leftSquare > rightSquare)
            {
                result[write--] = leftSquare;
                left++;
            }
            else
            {
                result[write--] = rightSquare;
                right--;
            }
        }

        return result;
    }
}

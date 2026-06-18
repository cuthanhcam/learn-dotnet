using System.Text;

namespace Dsa.Examples.ArraysStrings;

public static class ArraysStringsExample
{
    public static int[] BuildPrefixSums(ReadOnlySpan<int> values)
    {
        int[] prefix = new int[values.Length + 1];

        for (int i = 0; i < values.Length; i++)
        {
            prefix[i + 1] = prefix[i] + values[i];
        }

        return prefix;
    }

    public static int RangeSum(int[] prefixSums, int startInclusive, int endExclusive)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startInclusive);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(endExclusive, prefixSums.Length - 1);

        if (startInclusive > endExclusive)
        {
            throw new ArgumentException("Start must be less than or equal to end.");
        }

        return prefixSums[endExclusive] - prefixSums[startInclusive];
    }

    public static bool IsPalindromeIgnoringNonLetters(string text)
    {
        int left = 0;
        int right = text.Length - 1;

        while (left < right)
        {
            while (left < right && !char.IsLetterOrDigit(text[left]))
            {
                left++;
            }

            while (left < right && !char.IsLetterOrDigit(text[right]))
            {
                right--;
            }

            if (char.ToUpperInvariant(text[left]) != char.ToUpperInvariant(text[right]))
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    public static int LongestSubstringWithoutRepeatingCharacters(string text)
    {
        Dictionary<char, int> lastSeen = [];
        int best = 0;
        int windowStart = 0;

        for (int windowEnd = 0; windowEnd < text.Length; windowEnd++)
        {
            char current = text[windowEnd];

            if (lastSeen.TryGetValue(current, out int previousIndex) && previousIndex >= windowStart)
            {
                windowStart = previousIndex + 1;
            }

            lastSeen[current] = windowEnd;
            best = Math.Max(best, windowEnd - windowStart + 1);
        }

        return best;
    }

    public static int[] TwoSumSorted(ReadOnlySpan<int> sortedValues, int target)
    {
        int left = 0;
        int right = sortedValues.Length - 1;

        while (left < right)
        {
            int sum = sortedValues[left] + sortedValues[right];

            if (sum == target)
            {
                return [left, right];
            }

            if (sum < target)
            {
                left++;
            }
            else
            {
                right--;
            }
        }

        return [];
    }

    public static string ReverseWords(string sentence)
    {
        string[] words = sentence
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Array.Reverse(words);
        return string.Join(' ', words);
    }

    public static string CompressRuns(string text)
    {
        if (text.Length == 0)
        {
            return string.Empty;
        }

        StringBuilder builder = new();
        char current = text[0];
        int count = 1;

        for (int i = 1; i < text.Length; i++)
        {
            if (text[i] == current)
            {
                count++;
                continue;
            }

            builder.Append(current);
            builder.Append(count);
            current = text[i];
            count = 1;
        }

        builder.Append(current);
        builder.Append(count);

        string compressed = builder.ToString();
        return compressed.Length < text.Length ? compressed : text;
    }

    public static void Run()
    {
        int[] values = [3, -1, 4, 10, 2];
        int[] prefix = BuildPrefixSums(values);

        Console.WriteLine("Arrays and strings");
        Console.WriteLine($"Range sum [1, 4): {RangeSum(prefix, 1, 4)}");
        Console.WriteLine($"Palindrome check: {IsPalindromeIgnoringNonLetters("Never odd or even")}");
        Console.WriteLine($"Longest unique substring in 'pwwkew': {LongestSubstringWithoutRepeatingCharacters("pwwkew")}");
        Console.WriteLine($"Two sum indexes for 10: {string.Join(", ", TwoSumSorted([1, 3, 4, 6, 8], 10))}");
        Console.WriteLine($"Reverse words: {ReverseWords("  dotnet algorithms are fun  ")}");
        Console.WriteLine($"Compress runs: {CompressRuns("aaabbc")}");
    }
}

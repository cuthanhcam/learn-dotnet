namespace CSharpBasics.Exercises.Medium;

public static class Palindrome
{
    // String exercise with normalization.
    public static bool IsPalindrome(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        char[] chars = input
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray();

        int left = 0;
        int right = chars.Length - 1;

        while (left < right)
        {
            if (chars[left] != chars[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }
}

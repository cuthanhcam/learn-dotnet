namespace MemoryPerformance.Exercises;

public static class SpanExercises
{
    public static int[] ParseThreeNumbers(ReadOnlySpan<char> text)
    {
        Span<int> values = stackalloc int[3];
        int index = 0;
        ReadOnlySpan<char> remaining = text;

        while (true)
        {
            if (index >= values.Length)
            {
                throw new FormatException("Input must contain exactly three integer values.");
            }

            int commaIndex = remaining.IndexOf(',');
            ReadOnlySpan<char> token = commaIndex >= 0
                ? remaining[..commaIndex]
                : remaining;

            if (!int.TryParse(token, out values[index]))
            {
                throw new FormatException("Input must contain exactly three integer values.");
            }

            index++;

            if (commaIndex < 0)
            {
                break;
            }

            remaining = remaining[(commaIndex + 1)..];
        }

        if (index != values.Length)
        {
            throw new FormatException("Input must contain exactly three integer values.");
        }

        return values.ToArray();
    }

    public static string RemoveWhitespace(ReadOnlySpan<char> text)
    {
        Span<char> buffer = text.Length <= 128
            ? stackalloc char[text.Length]
            : new char[text.Length];

        int position = 0;
        foreach (char character in text)
        {
            if (!char.IsWhiteSpace(character))
            {
                buffer[position++] = character;
            }
        }

        return new string(buffer[..position]);
    }
}

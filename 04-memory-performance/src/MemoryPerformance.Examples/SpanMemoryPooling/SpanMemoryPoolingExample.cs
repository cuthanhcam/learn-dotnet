using System.Buffers;

namespace MemoryPerformance.Examples.SpanMemoryPooling;

/// <summary>
/// Demonstrates spans for slicing existing memory and ArrayPool for reusable temporary buffers.
/// </summary>
public static class SpanMemoryPoolingExample
{
    public static void Run()
    {
        Console.WriteLine($"Parsed CSV: {string.Join(", ", ParseThreeNumbers("10,20,30"))}");
        Console.WriteLine($"Normalized code: {NormalizeProductCode("  ab-123  ")}");
        Console.WriteLine($"Stack scratch sum: {StackallocSum(5)}");
        Console.WriteLine($"Pooled buffer checksum: {RentFillAndSum(32)}");
        Console.WriteLine($"Formatted id: {FormatOrderId(42)}");
    }

    public static int[] ParseThreeNumbers(ReadOnlySpan<char> text)
    {
        Span<int> values = stackalloc int[3];
        int index = 0;
        ReadOnlySpan<char> remaining = text;

        while (true)
        {
            if (index >= values.Length)
            {
                throw new FormatException("Expected exactly three numbers.");
            }

            int commaIndex = remaining.IndexOf(',');
            ReadOnlySpan<char> token = commaIndex >= 0
                ? remaining[..commaIndex]
                : remaining;

            if (!int.TryParse(token, out values[index]))
            {
                throw new FormatException("Input contains a non-numeric value.");
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
            throw new FormatException("Expected exactly three numbers.");
        }

        return values.ToArray();
    }

    public static string NormalizeProductCode(ReadOnlySpan<char> code)
    {
        code = code.Trim();

        Span<char> buffer = code.Length <= 64
            ? stackalloc char[code.Length]
            : new char[code.Length];

        int position = 0;
        foreach (char character in code)
        {
            if (character != '-')
            {
                buffer[position++] = char.ToUpperInvariant(character);
            }
        }

        return new string(buffer[..position]);
    }

    public static int StackallocSum(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Keep stackalloc buffers small.");
        }

        Span<int> values = stackalloc int[count];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = i + 1;
        }

        int sum = 0;
        foreach (int value in values)
        {
            sum += value;
        }

        return sum;
    }

    public static int RentFillAndSum(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        int[] rented = ArrayPool<int>.Shared.Rent(length);
        try
        {
            Span<int> slice = rented.AsSpan(0, length);
            for (int i = 0; i < slice.Length; i++)
            {
                slice[i] = i + 1;
            }

            int sum = 0;
            foreach (int value in slice)
            {
                sum += value;
            }

            slice.Clear();
            return sum;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }

    public static string FormatOrderId(int id)
    {
        Span<char> buffer = stackalloc char[16];
        if (!buffer.TryWrite($"ORD-{id:000000}", out int charsWritten))
        {
            throw new InvalidOperationException("Could not format order id.");
        }

        return new string(buffer[..charsWritten]);
    }
}

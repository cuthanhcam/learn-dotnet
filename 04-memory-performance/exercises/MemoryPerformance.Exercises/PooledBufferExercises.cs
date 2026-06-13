using System.Buffers;

namespace MemoryPerformance.Exercises;

public static class PooledBufferExercises
{
    public static string NormalizeKey(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        char[] rented = ArrayPool<char>.Shared.Rent(key.Length);
        try
        {
            int position = 0;
            foreach (char character in key.AsSpan().Trim())
            {
                if (character == ' ')
                {
                    continue;
                }

                rented[position++] = character == '-'
                    ? '_'
                    : char.ToUpperInvariant(character);
            }

            return new string(rented, 0, position);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented, clearArray: true);
        }
    }
}

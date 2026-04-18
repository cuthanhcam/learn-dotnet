namespace CSharpBasics.Exercises.Medium;

public static class PrimeNumbers
{
    public static List<int> GetPrimesUpTo(int limit)
    {
        if (limit < 2)
        {
            return new List<int>();
        }

        var isPrime = new bool[limit + 1];
        Array.Fill(isPrime, true);
        isPrime[0] = false;
        isPrime[1] = false;

        for (int i = 2; i * i <= limit; i++)
        {
            if (!isPrime[i])
            {
                continue;
            }

            for (int j = i * i; j <= limit; j += i)
            {
                isPrime[j] = false;
            }
        }

        var result = new List<int>();
        for (int i = 2; i <= limit; i++)
        {
            if (isPrime[i])
            {
                result.Add(i);
            }
        }

        return result;
    }
}

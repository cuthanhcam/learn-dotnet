namespace Dsa.Examples.RecursionBacktracking;

public static class RecursionBacktrackingExample
{
    public static int Factorial(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        return n <= 1 ? 1 : n * Factorial(n - 1);
    }

    public static int FibonacciMemoized(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        Dictionary<int, int> memo = [];
        return Solve(n);

        int Solve(int value)
        {
            if (value <= 1)
            {
                return value;
            }

            if (memo.TryGetValue(value, out int cached))
            {
                return cached;
            }

            int result = Solve(value - 1) + Solve(value - 2);
            memo[value] = result;
            return result;
        }
    }

    public static int[][] Subsets(int[] values)
    {
        List<int[]> result = [];
        List<int> path = [];

        Backtrack(0);
        return result.ToArray();

        void Backtrack(int index)
        {
            if (index == values.Length)
            {
                result.Add(path.ToArray());
                return;
            }

            Backtrack(index + 1);

            path.Add(values[index]);
            Backtrack(index + 1);
            path.RemoveAt(path.Count - 1);
        }
    }

    public static int[][] Permutations(int[] values)
    {
        List<int[]> result = [];
        List<int> path = [];
        bool[] used = new bool[values.Length];

        Backtrack();
        return result.ToArray();

        void Backtrack()
        {
            if (path.Count == values.Length)
            {
                result.Add(path.ToArray());
                return;
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (used[i])
                {
                    continue;
                }

                used[i] = true;
                path.Add(values[i]);
                Backtrack();
                path.RemoveAt(path.Count - 1);
                used[i] = false;
            }
        }
    }

    public static int[][] CombinationSum(int[] candidates, int target)
    {
        Array.Sort(candidates);
        List<int[]> result = [];
        List<int> path = [];

        Backtrack(0, target);
        return result.ToArray();

        void Backtrack(int startIndex, int remaining)
        {
            if (remaining == 0)
            {
                result.Add(path.ToArray());
                return;
            }

            for (int i = startIndex; i < candidates.Length; i++)
            {
                int candidate = candidates[i];

                if (candidate > remaining)
                {
                    break;
                }

                path.Add(candidate);
                Backtrack(i, remaining - candidate);
                path.RemoveAt(path.Count - 1);
            }
        }
    }

    public static void Run()
    {
        Console.WriteLine("Recursion and backtracking");
        Console.WriteLine($"5!: {Factorial(5)}");
        Console.WriteLine($"Fibonacci(10): {FibonacciMemoized(10)}");
        Console.WriteLine($"Subsets of [1,2]: {Subsets([1, 2]).Length}");
        Console.WriteLine($"Permutations of [1,2,3]: {Permutations([1, 2, 3]).Length}");
    }
}

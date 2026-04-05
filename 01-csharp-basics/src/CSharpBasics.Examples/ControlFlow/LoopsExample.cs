using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBasics.Examples.ControlFlow
{
    /// <summary>
    /// Comprehensive lesson for for/while/do-while/foreach loops.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Loop types:
    /// - for: Best for known iteration count
    /// - while: Best for condition-based iteration
    /// - do-while: Guarantees at least one execution
    /// - foreach: Best for iterating collections (no index needed)
    /// 
    /// Key topics:
    /// - Loop structure and control flow
    /// - Break and continue statements
    /// - Index vs element iteration
    /// - Nested loop patterns
    /// - Loop termination conditions
    /// 
    /// Best practices:
    /// - Use foreach for collections when index isn't needed
    /// - Use for when you need iteration counter
    /// - Use while for sentinel/condition-based loops
    /// - Prefer foreach over for when possible (simpler, fewer bugs)
    /// - Be careful with loop termination conditions to avoid infinite loops
    /// </summary>
    public static class LoopsExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} LoopsExample {new string('=', 5)}");

            PrintSection("FOR LOOP");
            DemoForLoop();

            PrintSection("WHILE LOOP");
            DemoWhileLoop();

            PrintSection("DO-WHILE LOOP");
            DemoDoWhileLoop();

            PrintSection("FOREACH LOOP");
            DemoForeachLoop();

            PrintSection("LOOP CONTROL (BREAK & CONTINUE)");
            DemoLoopControl();

            PrintSection("NESTED LOOPS");
            DemoNestedLoops();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Generates array of perfect squares using for loop.
        /// Demonstrates standard indexed iteration.
        /// </summary>
        public static int[] GenerateSquares(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");

            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = i * i;
            }

            return result;
        }

        /// <summary>
        /// Sums numbers using while loop.
        /// Demonstrates condition-based iteration with manual index management.
        /// </summary>
        public static int SumWithWhile(IReadOnlyList<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            int index = 0;
            int total = 0;
            while (index < numbers.Count)
            {
                total += numbers[index];
                index++;
            }

            return total;
        }

        /// <summary>
        /// Counts digits in a number using do-while loop.
        /// Guaranteed to execute at least once.
        /// Useful for digit/string processing.
        /// </summary>
        public static int CountDigitsWithDoWhile(int number)
        {
            int value = Math.Abs(number);
            int digits = 0;

            do
            {
                digits++;
                value /= 10;
            }
            while (value > 0);

            return digits;
        }

        /// <summary>
        /// Counts positive numbers using foreach loop.
        /// Demonstrates clean element-only iteration (no index).
        /// Best practice: use foreach when index isn't needed.
        /// </summary>
        public static int CountPositiveWithForeach(IEnumerable<int> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            int count = 0;
            foreach (int value in values)
            {
                if (value > 0)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Joins words using foreach loop.
        /// Demonstrates accumulating results from iteration.
        /// </summary>
        public static string JoinWithSeparator(IEnumerable<string> words, string separator)
        {
            if (words == null)
            {
                throw new ArgumentNullException(nameof(words));
            }

            if (separator == null)
            {
                throw new ArgumentNullException(nameof(separator));
            }

            List<string> list = [];
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                    list.Add(word.Trim());
            }

            return string.Join(separator, list);
        }

        /// <summary>
        /// Finds first element matching a condition.
        /// Demonstrates early exit from loop with break.
        /// </summary>
        public static int? FindFirstEven(IEnumerable<int> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            foreach (int value in values)
            {
                if (value % 2 == 0)
                    return value;  // Early exit
            }

            return null;  // Not found
        }

        /// <summary>
        /// Counts non-negative numbers.
        /// Demonstrates continue statement to skip iteration.
        /// </summary>
        public static int CountNonNegative(IEnumerable<int> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            int count = 0;
            foreach (int value in values)
            {
                if (value < 0)
                    continue;  // Skip negative numbers

                count++;
            }

            return count;
        }

        /// <summary>
        /// Generates multiplication table as 2D array.
        /// Demonstrates nested loops for matrix generation.
        /// </summary>
        public static int[,] GenerateMultiplicationTable(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be at least 1.");
            }

            int[,] table = new int[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    table[row, col] = (row + 1) * (col + 1);
                }
            }

            return table;
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates for loop with index-based iteration.
        /// </summary>
        private static void DemoForLoop()
        {
            Console.WriteLine("Generate squares (0² to 5²):");
            int[] squares = GenerateSquares(6);
            for (int i = 0; i < squares.Length; i++)
            {
                Console.WriteLine($"  {i}² = {squares[i]}");
            }
        }

        /// <summary>
        /// Demonstrates while loop with manual index management.
        /// </summary>
        private static void DemoWhileLoop()
        {
            int[] numbers = [1, 2, 3, 4, 5];
            Console.WriteLine($"Sum using while: {SumWithWhile(numbers)}");

            // Countdown example
            Console.Write("Countdown: ");
            int countdown = 5;
            while (countdown > 0)
            {
                Console.Write($"{countdown} ");
                countdown--;
            }
            Console.WriteLine("Blast off!");
        }

        /// <summary>
        /// Demonstrates do-while loop (always executes at least once).
        /// </summary>
        private static void DemoDoWhileLoop()
        {
            Console.WriteLine($"Digit count (12345): {CountDigitsWithDoWhile(12345)}");
            Console.WriteLine($"Digit count (9): {CountDigitsWithDoWhile(9)}");
            Console.WriteLine($"Digit count (0): {CountDigitsWithDoWhile(0)}");

            // Menu-like pattern
            int choice = 0;
            int attempts = 0;
            do
            {
                attempts++;
                choice = attempts > 1 ? 2 : 1;  // Simulate user input
            } while (choice != 2 && attempts < 2);
            Console.WriteLine($"Do-while executed {attempts} time(s)");
        }

        /// <summary>
        /// Demonstrates foreach loop (cleanest for collections).
        /// </summary>
        private static void DemoForeachLoop()
        {
            int[] values = [-5, 2, -1, 7, 4, -3, 9];
            Console.WriteLine($"Values: {string.Join(", ", values)}");
            Console.WriteLine($"Positive count: {CountPositiveWithForeach(values)}");

            string[] words = ["csharp", "loops", "iteration"];
            Console.WriteLine($"Joined: {JoinWithSeparator(words, " -> ")}");
        }

        /// <summary>
        /// Demonstrates break and continue statements.
        /// </summary>
        private static void DemoLoopControl()
        {
            int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

            // Break example
            int? firstEven = FindFirstEven(numbers);
            Console.WriteLine($"First even number: {firstEven}");

            // Continue example
            int[] testValues = [1, -2, 3, -4, 5];
            Console.WriteLine($"Non-negative count in {string.Join(", ", testValues)}: {CountNonNegative(testValues)}");
        }

        /// <summary>
        /// Demonstrates nested loops for 2D structures.
        /// </summary>
        private static void DemoNestedLoops()
        {
            Console.WriteLine("3x3 Multiplication table:");
            int[,] table = GenerateMultiplicationTable(3);

            for (int row = 0; row < table.GetLength(0); row++)
            {
                for (int col = 0; col < table.GetLength(1); col++)
                {
                    Console.Write($"{table[row, col]:D2} ");
                }
                Console.WriteLine();
            }
        }
    }
}

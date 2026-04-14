using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpBasics.Examples.Collections
{
    /// <summary>
    /// Comprehensive array lesson for C# fundamentals.
    /// Covers declaration, initialization, access, defaults, iteration,
    /// searching, copying, sorting, multidimensional arrays, and practical patterns.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Key topics:
    /// - Array declaration and initialization patterns
    /// - Index access (forward and backward)
    /// - Default values by type
    /// - Iteration techniques (for, foreach)
    /// - Search and filtering algorithms
    /// - Copy semantics (reference vs value)
    /// - Sorting algorithms (bubble sort, built-in)
    /// - Multidimensional and jagged arrays
    /// - Performance considerations
    /// 
    /// When to use arrays:
    /// - Fixed-size collections
    /// - High-performance iteration
    /// - Multidimensional data structures
    /// 
    /// When NOT to use arrays:
    /// - Dynamic size collections → use List<T>
    /// - Key-value storage → use Dictionary<K, V>
    /// - Complex queries → use LINQ with IEnumerable<T>
    /// </summary>
    public static class ArraysExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} ArraysExample {new string('=', 5)}");

            PrintSection("DECLARATION & INITIALIZATION");
            DemoDeclarationAndInitialization();

            PrintSection("ACCESSING ELEMENTS");
            DemoAccessingElements();

            PrintSection("DEFAULT VALUES");
            DemoDefaultValues();

            PrintSection("ITERATION");
            DemoIteration();

            PrintSection("ARRAY OPERATIONS");
            DemoOperations();

            PrintSection("SEARCHING");
            DemoSearching();

            PrintSection("SAFE INDEXING");
            DemoSafeIndexing();

            PrintSection("COPYING");
            DemoCopying();

            PrintSection("SORTING");
            DemoSorting();

            PrintSection("MULTI-DIMENSIONAL / JAGGED");
            DemoMultiDimensional();

            PrintSection("PRACTICAL EXAMPLES");
            DemoPracticalExamples();

            PrintSection("PERFORMANCE NOTES");
            DemoPerformanceNotes();

            Console.WriteLine();
        }

        public static string[] CreateWeekdays() => ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];

        /// <summary>
        /// Flattens a jagged array while preserving row-major order.
        /// </summary>
        public static int[] Flatten(int[][] jaggedArray)
        {
            if (jaggedArray is null)
            {
                throw new ArgumentNullException(nameof(jaggedArray));
            }

            List<int> flattened = [];
            foreach (int[] row in jaggedArray)
            {
                if (row is null)
                {
                    continue;
                }

                foreach (int value in row)
                {
                    flattened.Add(value);
                }
            }

            return [.. flattened];
        }

        /// <summary>
        /// Creates a matrix and fills it with sequential values.
        /// </summary>
        public static int[,] CreateMatrix(int rows, int columns)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);

            int[,] matrix = new int[rows, columns];
            int value = 1;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    matrix[r, c] = value++;
                }
            }

            return matrix;
        }

        public static int Sum(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            int total = 0;
            foreach (int value in values)
            {
                total += value;
            }

            return total;
        }

        public static double Average(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values.Length == 0)
            {
                return 0;
            }

            return (double)Sum(values) / values.Length;
        }

        public static int FindMax(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values.Length == 0)
            {
                throw new ArgumentException("Array cannot be empty.", nameof(values));
            }

            int max = values[0];
            foreach (int value in values)
            {
                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        public static int FindMin(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values.Length == 0)
            {
                throw new ArgumentException("Array cannot be empty.", nameof(values));
            }

            int min = values[0];
            foreach (int value in values)
            {
                if (value < min)
                {
                    min = value;
                }
            }

            return min;
        }

        public static int LinearSearch(int[] values, int target)
        {
            ArgumentNullException.ThrowIfNull(values);

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == target)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for a value in a sorted array using binary search.
        /// Returns index when found, otherwise -1.
        /// Time complexity: O(log n).
        /// </summary>
        public static int BinarySearchSorted(int[] sortedValues, int target)
        {
            ArgumentNullException.ThrowIfNull(sortedValues);

            if (!IsSortedAscending(sortedValues))
            {
                throw new ArgumentException("Array must be sorted in ascending order.", nameof(sortedValues));
            }

            int left = 0;
            int right = sortedValues.Length - 1;

            while (left <= right)
            {
                int middle = left + ((right - left) / 2);
                int value = sortedValues[middle];

                if (value == target)
                {
                    return middle;
                }

                if (value < target)
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

        /// <summary>
        /// Tries to safely get an item by index without throwing IndexOutOfRangeException.
        /// Useful when index may come from user input.
        /// </summary>
        public static bool TryGetAt<T>(T[] values, int index, out T? value)
        {
            ArgumentNullException.ThrowIfNull(values);

            if (index < 0 || index >= values.Length)
            {
                value = default;
                return false;
            }

            value = values[index];
            return true;
        }

        public static int[] ManualCopy(int[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            int[] copy = new int[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                copy[i] = source[i];
            }

            return copy;
        }

        public static void BubbleSort(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            for (int i = 0; i < values.Length - 1; i++)
            {
                bool swapped = false;

                for (int j = 0; j < values.Length - 1 - i; j++)
                {
                    if (values[j] > values[j + 1])
                    {
                        (values[j], values[j + 1]) = (values[j + 1], values[j]);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break;
                }
            }
        }

        public static void ReverseInPlace(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            for (int i = 0; i < values.Length / 2; i++)
            {
                int opposite = values.Length - 1 - i;
                (values[i], values[opposite]) = (values[opposite], values[i]);
            }
        }

        public static bool IsSortedAscending(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < values[i - 1])
                {
                    return false;
                }
            }

            return true;
        }

        public static int CountOccurrences(int[] values, int target)
        {
            ArgumentNullException.ThrowIfNull(values);

            int count = 0;
            foreach (int value in values)
            {
                if (value == target)
                {
                    count++;
                }
            }

            return count;
        }

        public static int[] DistinctPreserveOrder(int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            HashSet<int> seen = [];
            List<int> result = [];
            foreach (int value in values)
            {
                if (seen.Add(value))
                {
                    result.Add(value);
                }
            }

            return [.. result];
        }

        public static void RotateRight(int[] values, int rotationCount)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values.Length == 0)
            {
                return;
            }

            int k = rotationCount % values.Length;
            if (k < 0)
            {
                k += values.Length;
            }

            if (k == 0)
            {
                return;
            }

            int[] copy = ManualCopy(values);
            for (int i = 0; i < values.Length; i++)
            {
                int newIndex = (i + k) % values.Length;
                values[newIndex] = copy[i];
            }
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine(title);
        }

        private static void DemoDeclarationAndInitialization()
        {
            int[] numbers;
            numbers = new int[5];

            int[] scores = new int[3];
            int[] ages = [25, 30, 35, 40];

            Console.WriteLine($"numbers length: {numbers.Length}");
            Console.WriteLine($"scores length: {scores.Length}");
            Console.WriteLine($"ages: {string.Join(", ", ages)}");
            Console.WriteLine($"weekdays: {string.Join(", ", CreateWeekdays())}");
        }

        private static void DemoAccessingElements()
        {
            int[] ages = [25, 30, 35, 40];

            Console.WriteLine($"first: {ages[0]}");
            Console.WriteLine($"second: {ages[1]}");
            Console.WriteLine($"last: {ages[^1]}");

            ages[0] = 26;
            Console.WriteLine($"modified first: {ages[0]}");
        }

        private static void DemoDefaultValues()
        {
            int[] defaultInts = new int[3];
            double[] defaultDoubles = new double[3];
            bool[] defaultBooleans = new bool[3];
            string[] defaultStrings = new string[3];

            Console.WriteLine($"int default: {defaultInts[0]}");
            Console.WriteLine($"double default: {defaultDoubles[0]}");
            Console.WriteLine($"bool default: {defaultBooleans[0]}");
            Console.WriteLine($"string default: {defaultStrings[0] ?? "null"}");
        }

        private static void DemoIteration()
        {
            int[] nums = [10, 20, 30, 40, 50];

            Console.WriteLine("traditional for:");
            for (int i = 0; i < nums.Length; i++)
            {
                Console.WriteLine($"index {i}: {nums[i]}");
            }

            Console.WriteLine("foreach:");
            foreach (int num in nums)
            {
                Console.WriteLine($"value: {num}");
            }
        }

        private static void DemoOperations()
        {
            int[] values = [5, 10, 15, 20, 25];

            Console.WriteLine($"sum: {Sum(values)}");
            Console.WriteLine($"average: {Average(values):0.00}");
            Console.WriteLine($"max: {FindMax(values)}");
            Console.WriteLine($"min: {FindMin(values)}");
        }

        private static void DemoSearching()
        {
            int[] searchArray = [10, 20, 30, 40, 50, 60, 70];
            int target = 50;

            int linearIndex = LinearSearch(searchArray, target);
            int binaryIndex = BinarySearchSorted(searchArray, target);
            int frameworkIndex = Array.BinarySearch(searchArray, target);

            Console.WriteLine($"linear search index: {linearIndex} (O(n))");
            Console.WriteLine($"binary search index: {binaryIndex} (O(log n), requires sorted array)");
            Console.WriteLine($"Array.BinarySearch index: {frameworkIndex}");
        }

        private static void DemoSafeIndexing()
        {
            string[] names = ["Cam", "Linh", "An"];

            if (TryGetAt(names, 1, out string? secondName))
            {
                Console.WriteLine($"index 1 => {secondName}");
            }

            bool hasIndex5 = TryGetAt(names, 5, out string? missing);
            Console.WriteLine(hasIndex5
                ? $"index 5 => {missing}"
                : "index 5 is out of range (handled safely)");
        }

        private static void DemoCopying()
        {
            int[] original = [1, 2, 3, 4, 5];

            int[] wrongCopy = original;
            wrongCopy[0] = 999;
            Console.WriteLine($"original[0] after reference copy: {original[0]}");

            original[0] = 1;
            int[] manualCopy = ManualCopy(original);
            manualCopy[0] = 99;
            Console.WriteLine($"original[0] after manual copy: {original[0]}");

            int[] systemCopy = new int[original.Length];
            Array.Copy(original, 0, systemCopy, 0, original.Length);

            int[] utilCopy = original.ToArray();
            Console.WriteLine($"system copy: {string.Join(", ", systemCopy)}");
            Console.WriteLine($"linq copy: {string.Join(", ", utilCopy)}");
        }

        private static void DemoSorting()
        {
            int[] unsorted = [5, 2, 8, 1, 9, 3];
            int[] bubbleSorted = ManualCopy(unsorted);
            BubbleSort(bubbleSorted);

            int[] builtInSorted = ManualCopy(unsorted);
            Array.Sort(builtInSorted);

            Console.WriteLine($"unsorted: {string.Join(", ", unsorted)}");
            Console.WriteLine($"bubble sort: {string.Join(", ", bubbleSorted)}");
            Console.WriteLine($"Array.Sort: {string.Join(", ", builtInSorted)}");
            Console.WriteLine($"is sorted (Array.Sort result): {IsSortedAscending(builtInSorted)}");
        }

        private static void DemoMultiDimensional()
        {
            int[,] matrix = CreateMatrix(3, 3);
            Console.WriteLine("matrix:");
            for (int r = 0; r < matrix.GetLength(0); r++)
            {
                for (int c = 0; c < matrix.GetLength(1); c++)
                {
                    Console.Write($"{matrix[r, c],3}");
                }

                Console.WriteLine();
            }

            int[][] jagged =
            [
                [1, 2],
                [3, 4, 5],
                [6]
            ];

            Console.WriteLine($"jagged flattened: {string.Join(", ", Flatten(jagged))}");
        }

        private static void DemoPracticalExamples()
        {
            int[] toReverse = [1, 2, 3, 4, 5];
            ReverseInPlace(toReverse);
            Console.WriteLine($"reversed: {string.Join(", ", toReverse)}");

            int[] countArray = [1, 2, 3, 2, 4, 2, 5];
            Console.WriteLine($"occurrences of 2: {CountOccurrences(countArray, 2)}");

            int[] withDuplicates = [1, 2, 2, 3, 3, 3, 4, 5, 5];
            Console.WriteLine($"unique values: {string.Join(", ", DistinctPreserveOrder(withDuplicates))}");

            int[] toRotate = [1, 2, 3, 4, 5];
            RotateRight(toRotate, 2);
            Console.WriteLine($"rotate right by 2: {string.Join(", ", toRotate)}");
        }

        private static void DemoPerformanceNotes()
        {
            Console.WriteLine("for-loop: best when you need index and maximum performance.");
            Console.WriteLine("foreach: cleaner and safer for read-only iteration.");
            Console.WriteLine("LinearSearch: O(n), use for unsorted arrays.");
            Console.WriteLine("BinarySearch: O(log n), use only for sorted arrays.");
            Console.WriteLine("Array.Sort: preferred in production over educational bubble sort.");
            Console.WriteLine("Arrays are fixed-size; choose List<T> when size changes frequently.");
        }
    }
}

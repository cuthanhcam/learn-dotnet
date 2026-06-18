using Dsa.Exercises;

Console.WriteLine("DSA Exercises");
Console.WriteLine($"Move zeroes: {string.Join(", ", ArraysStringsExercises.MoveZeroesToEnd([0, 1, 0, 3, 12]))}");
Console.WriteLine($"Anagram: {ArraysStringsExercises.AreAnagrams("listen", "silent")}");
Console.WriteLine($"Max window sum: {ArraysStringsExercises.MaxSubarraySumOfSizeK([2, 1, 5, 1, 3, 2], 3)}");
Console.WriteLine($"Remove adjacent duplicates: {StacksQueuesExercises.RemoveAdjacentDuplicates("abbaca")}");
Console.WriteLine($"Longest consecutive sequence: {HashTableExercises.LongestConsecutiveSequence([100, 4, 200, 1, 3, 2])}");

using System;

namespace ArraysAndStrings
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example usage of TwoSumSolution
            var twoSumSolution = new TwoSumSolution();
            int[] nums = { 2, 7, 11, 15 };
            int target = 9;
            int[] result = twoSumSolution.TwoSum(nums, target);
            Console.WriteLine($"Two Sum Result: [{string.Join(", ", result)}]");

            // Example usage of ReverseStringSolution
            var reverseStringSolution = new ReverseStringSolution();
            char[] str = { 'h', 'e', 'l', 'l', 'o' };
            reverseStringSolution.ReverseString(str);
            Console.WriteLine($"Reversed String: [{string.Join(", ", str)}]");
        }
    }
}
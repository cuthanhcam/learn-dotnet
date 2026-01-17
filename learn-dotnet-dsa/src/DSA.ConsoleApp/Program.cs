using DSA.Core.Algorithms.Searching;

namespace DSA.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Binary Search Demo");

            int[] nums = { 1, 3, 5, 7, 9, 11, 13 };
            int target = 11;

            for (int i = 0; i <= nums.Length - 1; i++)
            {
                Console.Write($"{nums[i]} ");
            }

            Console.WriteLine($"\nTarget: {target}");
            
            // Iterative
            int index = BinarySearch.BinarySearchIterative(nums, target);

            if (index != -1)
            {
                Console.WriteLine($"Iterative search resul: index = {index}");
            }
            else
            {
                Console.WriteLine($"{target} not found");
            }

            // Recursive
            int recursiveIndex = BinarySearch.BinarySearchRecursive(nums, target);

            Console.WriteLine($"Recursive search result: index = {recursiveIndex}");
        }
    }
}
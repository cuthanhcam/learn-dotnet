using DSA.Core.Algorithms.Searching;

namespace DSA.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Binary Search Demo");

            int[] arr = { 1, 3, 5, 7, 9, 11, 13 };
            int target = 11;

            int index = BinarySearch.BinarySearchIterative(arr, target);

            if (index != -1)
            {
                Console.WriteLine($"Found: {target} at index {index}");
            }
            else
            {
                Console.WriteLine($"{target} not found");
            }
        }
    }
}
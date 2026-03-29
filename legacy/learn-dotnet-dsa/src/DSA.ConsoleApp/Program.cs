using DSA.Core.Algorithms.Searching;
using DSA.Core.DataStructures.Arrays;

namespace DSA.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Data Structures Array Remove Duplicates Demo");

            int[] nums = { 1, 1, 2, 2, 3, 4 };
 
            Console.WriteLine($"Two pointer result: {TwoPointers.RemoveDuplicates(nums)}");
            Console.WriteLine($"Two pointer result: {TwoPointers.RemoveDuplicatesHashSet(nums)}");
        }
    }
}
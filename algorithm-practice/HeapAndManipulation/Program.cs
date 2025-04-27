using System;

namespace HeapAndManipulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test KthLargestElement
            int[] nums1 = { 3, 2, 1, 5, 6, 4 };
            int k1 = 2;
            var kthLargest = new KthLargestElementSolution();
            Console.WriteLine("Kth Largest: " + kthLargest.FindKthLargest(nums1, k1)); // Output: 5

            // Test TopKFrequent
            int[] nums2 = { 1, 1, 1, 2, 2, 3 };
            int k2 = 2;
            var topKFrequent = new TopKFrequentSolution();
            int[] result = topKFrequent.TopKFrequent(nums2, k2);
            Console.WriteLine("Top K Frequent: [" + string.Join(",", result) + "]"); // Output: [1,2]
        }
    }
}
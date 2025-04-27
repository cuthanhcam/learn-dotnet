using System.Collections.Generic;

namespace HeapAndManipulation
{
    public class KthLargestElementSolution
    {
        public int FindKthLargest(int[] nums, int k)
        {
            PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>();

            foreach (int num in nums)
            {
                minHeap.Enqueue(num, num);
                if (minHeap.Count > k)
                {
                    minHeap.Dequeue();
                }
            }

            return minHeap.Peek();
        }

        // QuickSelect algorithm for finding the kth largest element
        public int FindKthLargestQuickSelect(int[] nums, int k)
        {
            int left = 0, right = nums.Length - 1;
            k = nums.Length - k;
            while (left <= right)
            {
                int pivotIndex = Partition(nums, left, right);
                if (pivotIndex == k) return nums[k];
                else if (pivotIndex < k) left = pivotIndex + 1;
                else right = pivotIndex - 1;
            }
            return nums[left];
        }

        private int Partition(int[] nums, int left, int right)
        {
            int pivot = nums[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                if (nums[j] <= pivot)
                {
                    i++;
                    (nums[i], nums[j]) = (nums[j], nums[i]);
                }
            }
            (nums[i + 1], nums[right]) = (nums[right], nums[i + 1]);
            return i + 1;
        }
    }
}
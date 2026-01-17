using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA.Core.Algorithms.Searching
{
    /// <summary>
    /// Binary Search Algorithm
    /// 
    /// Requirements:
    /// - Input numsay must be sorted in ascending order.
    /// 
    /// Time Complexity: O(log n)
    /// Space Complexity: O(1)
    /// </summary>
    public static class BinarySearch
    {
        // Iterative implementation of Binary Search
        public static int BinarySearchIterative(int[] nums, int target)
        {
            if (nums == null || nums.Length == 0)
            {
                return -1;
            }

            int left = 0;
            int right = nums.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (nums[mid] == target)
                {
                    return mid;
                }
                else if (nums[mid] < target)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }

        // Recursive implementation of Binary Search
        public static int BinarySearchRecursive(int[] nums, int target)
        {
            if (nums == null || nums.Length == 0)
            {
                return -1;
            }

            return BinarySearchRecursive(nums, target, 0, nums.Length - 1);
        }

        private static int BinarySearchRecursive(int[] nums, int target, int left, int right)
        {
            if (left > right)
            {
                return -1;
            }

            int mid = left + (right - left) / 2;

            if (nums[mid] == target)
            {
                return mid;
            }

            if (nums[mid] < target)
            {
                return BinarySearchRecursive(nums, target, mid + 1, right);
            }

            return BinarySearchRecursive(nums, target, left, mid - 1);
        }

    }
}

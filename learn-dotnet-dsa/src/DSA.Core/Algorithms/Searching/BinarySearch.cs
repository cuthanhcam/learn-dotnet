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
    /// - Input array must be sorted in ascending order.
    /// 
    /// Time Complexity: O(log n)
    /// Space Complexity: O(1)
    /// </summary>
    public static class BinarySearch
    {
        // Iterative implementation of Binary Search
        public static int BinarySearchIterative(int[] arr, int target)
        {
            int low = 0;
            int high = arr.Length - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;

                if (arr[mid] == target)
                {
                    return mid;
                }
                else if (arr[mid] < target)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return -1;
        }

        // Recursive implementation of Binary Search
    }
}

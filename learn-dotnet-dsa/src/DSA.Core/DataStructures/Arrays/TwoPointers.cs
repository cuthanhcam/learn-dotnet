using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA.Core.DataStructures.Arrays
{
    public static class TwoPointers
    {
        // Two Pointers - yêu cầu mảng đã sắp xếp
        public static int RemoveDuplicates(int[] nums)
        {
            if (nums.Length == 0) 
                return 0;

            int index = 1;

            for (int i = 1; i < nums.Length; i++)
            {
                // Nếu khác số trước đó thì là số mới
                if (nums[i] != nums[i - 1])
                {
                    nums[index] = nums[i];
                    index++;
                }
            }

            return index;
        }

        // HashSet - không yêu cầu mảng sắp xếp
        public static int RemoveDuplicatesHashSet(int[] nums)
        {
            HashSet<int> set = new HashSet<int>();

            int index = 0;

            for (int i = 0; i < nums.Length; i++)
            {
                if (set.Add(nums[i]))
                {
                    nums[index] = nums[i];
                    index++;
                }
            }

            return index;
        }
    }
}

namespace DSA.Core.DataStructures.Arrays
{
    public static class Searching
    {
        // Linear Searching
        public static int LinearSearch(int[] nums, int target)
        {
            for (int i = 0; i < nums.Length - 1; i++)
            {
                if (nums[i] == target)
                {
                    return i;
                }
            }

            return -1;
        }

        // Binary Searching
        public static int BinarySearch(int[] nums, int target)
        {
            int left = 0;
            int right = nums.Length - 1;

            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (target == nums[mid])
                {
                    return mid;
                }
                else if (target > nums[mid])
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

    }
}

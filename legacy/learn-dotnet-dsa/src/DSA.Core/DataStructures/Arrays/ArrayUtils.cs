namespace DSA.Core.DataStructures.Arrays
{
    public static class ArrayUtils
    {
        // FindMax
        public static int FindMax(int[] nums)
        {
            int max = nums[0];
            for (int i = 1; i < nums.Length; i++)
            {
                if (nums[i] > max)
                {
                    max = nums[i];
                }
            }

            return max;
        }

        // FindMin
        public static int FindMix(int[] nums)
        {
            int min = nums[0];
            for (int i = 1; i < nums.Length; i++)
            {
                if (nums[i] < min)
                {
                    min = nums[i];
                }
            }

            return min;
        }

        // Sum
        public static int Sum(int[] nums)
        {
            int sum = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                sum += nums[i];
            }

            return sum;
        }

        // Reverse
        public static void Reverse(int[] nums)
        {
            int left = 0;
            int right = nums.Length - 1;

            while (left < right)
            {
                (nums[left], nums[right]) = (nums[right], nums[left]); // tuple deconstructuon

                //int temp = nums[left];
                //nums[left] = nums[right];
                //nums[right] = temp;

                left++;
                right--;
            }
        }
    }
}

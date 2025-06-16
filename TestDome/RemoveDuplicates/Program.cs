namespace RemoveDuplicates
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            int[] nums = { 1, 1, 2, 2, 3, 3, 3, 4, 4, 5, 6, 6 };
            Console.WriteLine(RemoveDuplicates(nums));
        }

        public static int RemoveDuplicates(int[] nums)
        {
            if (nums.Length == 0) return 0;
            int i = 0;
            for (int j = 1; j < nums.Length; j++)
            {
                if (nums[j] != nums[i])
                {
                    i++;
                    nums[i] = nums[j];
                }
            }
            return i + 1;
        }
    }
}

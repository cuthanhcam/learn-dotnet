using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArraysAndStrings
{
    public class TwoSumSolution
    {
        public int[] TwoSum(int[] nums, int target)
        {
            Dictionary<int, int> numDict = new Dictionary<int, int>();

            for (int i = 0; i < nums.Length; i++)
            {
                int complement = target - nums[i];

                if (numDict.ContainsKey(complement))
                {
                    return new int[] { numDict[complement], i };
                }

                if (!numDict.ContainsKey(nums[i]))
                {
                    numDict[nums[i]] = i;
                }
            }

            return new int[0]; // Return an empty array if no solution is found
        }
    }
}
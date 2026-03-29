using System;
using System.Collections.Generic;

namespace DynamicProgramming
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test ClimbingStairs
            var climbingStairs = new ClimbingStairsSolution();
            Console.WriteLine("Climbing Stairs: " + climbingStairs.ClimbStairs(3)); // Output: 3

            // Test CombinationSum
            var combinationSum = new CombinationSumSolution();
            int[] candidates = { 2, 3, 6, 7 };
            int target = 7;
            var combinations = combinationSum.CombinationSum(candidates, target);
            Console.WriteLine("Combination Sum:");
            foreach (var combo in combinations)
            {
                Console.WriteLine("[" + string.Join(",", combo) + "]");
            }
            // Output: [2,2,3], [7]
        }
    }
}
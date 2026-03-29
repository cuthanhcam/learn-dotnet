using System;

namespace TreesAndGraphs
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test MaxDepth
            TreeNode root = new TreeNode(3, new TreeNode(9), new TreeNode(20, new TreeNode(15), new TreeNode(7)));
            var maxDepth = new MaxDepthSolution();
            Console.WriteLine("Max Depth: " + maxDepth.MaxDepth(root)); // Output: 3

            // Test NumberOfIslands
            char[][] grid = new char[][] {
                new char[] { '1', '1', '0', '0', '0' },
                new char[] { '1', '1', '0', '0', '0' },
                new char[] { '0', '0', '1', '0', '0' },
                new char[] { '0', '0', '0', '1', '1' }
            };
            var numIslands = new NumberOfIslandsSolution();
            Console.WriteLine("Number of Islands: " + numIslands.NumIslands(grid)); // Output: 3
        }
    }
}
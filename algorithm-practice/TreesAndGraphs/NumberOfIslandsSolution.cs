using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreesAndGraphs
{
    public class NumberOfIslandsSolution
    {
        public int NumIslands(char[][] grid)
        {
            if (grid == null || grid.Length == 0) return 0;
            int islands = 0;

            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[0].Length; j++)
                {
                    if (grid[i][j] == '1')
                    {
                        islands++;
                        DFS(grid, i, j);
                    }
                }
            }

            return islands;
        }

        private void DFS(char[][] grid, int i, int j)
        {
            if (i < 0 || i >= grid.Length || j < 0 || j >= grid[0].Length || grid[i][j] != '1')
                return;

            grid[i][j] = '0'; // Đánh dấu đã thăm
            DFS(grid, i + 1, j);
            DFS(grid, i - 1, j);
            DFS(grid, i, j + 1);
            DFS(grid, i, j - 1);
        }

        // Sử dụng BFS thay vì DFS
        private void BFS(char[][] grid, int i, int j)
        {
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((i, j));
            grid[i][j] = '0';

            int[] directions = { -1, 0, 1, 0, -1 };
            while (queue.Count > 0)
            {
                var (row, col) = queue.Dequeue();
                for (int d = 0; d < 4; d++)
                {
                    int r = row + directions[d];
                    int c = col + directions[d + 1];
                    if (r >= 0 && r < grid.Length && c >= 0 && c < grid[0].Length && grid[r][c] == '1')
                    {
                        queue.Enqueue((r, c));
                        grid[r][c] = '0';
                    }
                }
            }
        }
    }
}
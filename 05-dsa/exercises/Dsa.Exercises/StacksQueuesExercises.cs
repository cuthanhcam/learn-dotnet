namespace Dsa.Exercises;

public static class StacksQueuesExercises
{
    public static string RemoveAdjacentDuplicates(string text)
    {
        Stack<char> stack = [];

        foreach (char character in text)
        {
            if (stack.Count > 0 && stack.Peek() == character)
            {
                stack.Pop();
            }
            else
            {
                stack.Push(character);
            }
        }

        char[] result = stack.Reverse().ToArray();
        return new string(result);
    }

    public static int CountIslands(char[][] grid)
    {
        if (grid.Length == 0)
        {
            return 0;
        }

        int rows = grid.Length;
        int cols = grid[0].Length;
        int islands = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row][col] == '1')
                {
                    islands++;
                    SinkIsland(grid, row, col);
                }
            }
        }

        return islands;
    }

    private static void SinkIsland(char[][] grid, int startRow, int startCol)
    {
        Queue<(int Row, int Col)> queue = [];
        queue.Enqueue((startRow, startCol));
        grid[startRow][startCol] = '0';

        ReadOnlySpan<(int Row, int Col)> directions =
        [
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1)
        ];

        while (queue.Count > 0)
        {
            (int row, int col) = queue.Dequeue();

            foreach ((int rowDelta, int colDelta) in directions)
            {
                int nextRow = row + rowDelta;
                int nextCol = col + colDelta;

                if (nextRow < 0 || nextRow >= grid.Length || nextCol < 0 || nextCol >= grid[0].Length)
                {
                    continue;
                }

                if (grid[nextRow][nextCol] != '1')
                {
                    continue;
                }

                grid[nextRow][nextCol] = '0';
                queue.Enqueue((nextRow, nextCol));
            }
        }
    }
}

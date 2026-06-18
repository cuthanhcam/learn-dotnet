namespace Dsa.Exercises;

public static class RecursionBacktrackingExercises
{
    public static string[] GenerateParentheses(int pairCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pairCount);
        List<string> result = [];
        char[] path = new char[pairCount * 2];

        Backtrack(0, 0, 0);
        return result.ToArray();

        void Backtrack(int index, int openUsed, int closeUsed)
        {
            if (index == path.Length)
            {
                result.Add(new string(path));
                return;
            }

            if (openUsed < pairCount)
            {
                path[index] = '(';
                Backtrack(index + 1, openUsed + 1, closeUsed);
            }

            if (closeUsed < openUsed)
            {
                path[index] = ')';
                Backtrack(index + 1, openUsed, closeUsed + 1);
            }
        }
    }

    public static bool WordExists(char[][] board, string word)
    {
        if (word.Length == 0)
        {
            return true;
        }

        for (int row = 0; row < board.Length; row++)
        {
            for (int col = 0; col < board[row].Length; col++)
            {
                if (Search(row, col, 0))
                {
                    return true;
                }
            }
        }

        return false;

        bool Search(int row, int col, int index)
        {
            if (index == word.Length)
            {
                return true;
            }

            if (row < 0 || row >= board.Length || col < 0 || col >= board[row].Length)
            {
                return false;
            }

            if (board[row][col] != word[index])
            {
                return false;
            }

            char original = board[row][col];
            board[row][col] = '#';

            bool found =
                Search(row - 1, col, index + 1) ||
                Search(row + 1, col, index + 1) ||
                Search(row, col - 1, index + 1) ||
                Search(row, col + 1, index + 1);

            board[row][col] = original;
            return found;
        }
    }
}

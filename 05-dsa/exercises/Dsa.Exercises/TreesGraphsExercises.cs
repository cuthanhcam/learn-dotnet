namespace Dsa.Exercises;

public static class TreesGraphsExercises
{
    public sealed class Node(int value)
    {
        public int Value { get; set; } = value;

        public Node? Left { get; set; }

        public Node? Right { get; set; }
    }

    public static bool IsValidBinarySearchTree(Node? root)
    {
        return IsValid(root, long.MinValue, long.MaxValue);

        static bool IsValid(Node? node, long minExclusive, long maxExclusive)
        {
            if (node is null)
            {
                return true;
            }

            if (node.Value <= minExclusive || node.Value >= maxExclusive)
            {
                return false;
            }

            return IsValid(node.Left, minExclusive, node.Value) &&
                   IsValid(node.Right, node.Value, maxExclusive);
        }
    }

    public static int CountConnectedComponents(IReadOnlyDictionary<int, int[]> graph)
    {
        HashSet<int> visited = [];
        int components = 0;

        foreach (int node in graph.Keys)
        {
            if (!visited.Add(node))
            {
                continue;
            }

            components++;
            Queue<int> queue = [];
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                foreach (int neighbor in graph[current])
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return components;
    }
}

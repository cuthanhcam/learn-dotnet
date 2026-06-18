namespace Dsa.Examples.TreesGraphs;

public static class TreesGraphsExample
{
    public static int MaxDepth(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return 0;
        }

        return 1 + Math.Max(MaxDepth(root.Left), MaxDepth(root.Right));
    }

    public static int[] InOrder(BinaryTreeNode<int>? root)
    {
        List<int> result = [];
        Traverse(root, result);
        return result.ToArray();

        static void Traverse(BinaryTreeNode<int>? node, List<int> output)
        {
            if (node is null)
            {
                return;
            }

            Traverse(node.Left, output);
            output.Add(node.Value);
            Traverse(node.Right, output);
        }
    }

    public static int[] LevelOrder(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return [];
        }

        Queue<BinaryTreeNode<int>> queue = [];
        List<int> result = [];
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            BinaryTreeNode<int> node = queue.Dequeue();
            result.Add(node.Value);

            if (node.Left is not null)
            {
                queue.Enqueue(node.Left);
            }

            if (node.Right is not null)
            {
                queue.Enqueue(node.Right);
            }
        }

        return result.ToArray();
    }

    public static string[] DepthFirstGraph(IReadOnlyDictionary<string, string[]> graph, string start)
    {
        if (!graph.ContainsKey(start))
        {
            return [];
        }

        List<string> result = [];
        HashSet<string> visited = [];

        Visit(start);
        return result.ToArray();

        void Visit(string node)
        {
            if (!visited.Add(node))
            {
                return;
            }

            result.Add(node);

            foreach (string neighbor in graph[node])
            {
                Visit(neighbor);
            }
        }
    }

    public static string[] ShortestPathUnweighted(IReadOnlyDictionary<string, string[]> graph, string start, string target)
    {
        if (!graph.ContainsKey(start) || !graph.ContainsKey(target))
        {
            return [];
        }

        Queue<string> queue = [];
        HashSet<string> visited = [start];
        Dictionary<string, string?> previous = new() { [start] = null };
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            string node = queue.Dequeue();

            if (node == target)
            {
                return ReconstructPath(previous, target);
            }

            foreach (string neighbor in graph[node])
            {
                if (visited.Add(neighbor))
                {
                    previous[neighbor] = node;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return [];
    }

    public static BinaryTreeNode<int> SampleTree()
    {
        return new BinaryTreeNode<int>(4)
        {
            Left = new BinaryTreeNode<int>(2)
            {
                Left = new BinaryTreeNode<int>(1),
                Right = new BinaryTreeNode<int>(3)
            },
            Right = new BinaryTreeNode<int>(6)
            {
                Left = new BinaryTreeNode<int>(5),
                Right = new BinaryTreeNode<int>(7)
            }
        };
    }

    public static void Run()
    {
        BinaryTreeNode<int> root = SampleTree();
        Dictionary<string, string[]> graph = new()
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = ["E"],
            ["D"] = ["F"],
            ["E"] = ["F"],
            ["F"] = []
        };

        Console.WriteLine("Trees and graphs");
        Console.WriteLine($"Tree max depth: {MaxDepth(root)}");
        Console.WriteLine($"Tree inorder: {string.Join(", ", InOrder(root))}");
        Console.WriteLine($"Shortest path A -> F: {string.Join(" -> ", ShortestPathUnweighted(graph, "A", "F"))}");
    }

    private static string[] ReconstructPath(Dictionary<string, string?> previous, string target)
    {
        Stack<string> path = [];
        string? current = target;

        while (current is not null)
        {
            path.Push(current);
            current = previous[current];
        }

        return path.ToArray();
    }
}

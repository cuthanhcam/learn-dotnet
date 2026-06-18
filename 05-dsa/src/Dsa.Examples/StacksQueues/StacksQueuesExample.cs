namespace Dsa.Examples.StacksQueues;

public static class StacksQueuesExample
{
    public static bool IsValidParentheses(string text)
    {
        Stack<char> stack = [];

        foreach (char character in text)
        {
            if (character is '(' or '[' or '{')
            {
                stack.Push(character);
                continue;
            }

            if (character is not (')' or ']' or '}'))
            {
                continue;
            }

            if (stack.Count == 0)
            {
                return false;
            }

            char opening = stack.Pop();

            if (!Matches(opening, character))
            {
                return false;
            }
        }

        return stack.Count == 0;
    }

    public static int[] NextGreaterElements(ReadOnlySpan<int> values)
    {
        int[] result = Enumerable.Repeat(-1, values.Length).ToArray();
        Stack<int> indexes = [];

        for (int i = 0; i < values.Length; i++)
        {
            while (indexes.Count > 0 && values[i] > values[indexes.Peek()])
            {
                int index = indexes.Pop();
                result[index] = values[i];
            }

            indexes.Push(i);
        }

        return result;
    }

    public static string[] BreadthFirstLevels(IReadOnlyDictionary<string, string[]> graph, string start)
    {
        if (!graph.ContainsKey(start))
        {
            return [];
        }

        Queue<(string Node, int Level)> queue = [];
        HashSet<string> visited = [start];
        List<string> result = [];

        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            (string node, int level) = queue.Dequeue();
            result.Add($"{level}:{node}");

            foreach (string neighbor in graph[node])
            {
                if (visited.Add(neighbor))
                {
                    queue.Enqueue((neighbor, level + 1));
                }
            }
        }

        return result.ToArray();
    }

    public static void Run()
    {
        TwoStackQueue<string> queue = new();
        queue.Enqueue("first");
        queue.Enqueue("second");
        queue.Enqueue("third");

        Console.WriteLine("Stacks and queues");
        Console.WriteLine($"Valid parentheses: {IsValidParentheses("({[]})")}");
        Console.WriteLine($"Next greater: {string.Join(", ", NextGreaterElements([2, 1, 2, 4, 3]))}");
        Console.WriteLine($"Two-stack queue dequeue: {queue.Dequeue()}, {queue.Dequeue()}");
    }

    private static bool Matches(char opening, char closing)
    {
        return (opening, closing) is ('(', ')') or ('[', ']') or ('{', '}');
    }
}

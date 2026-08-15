namespace Dsa.Examples.TreesGraphs;

public sealed class PrefixTrie
{
    private readonly Node _root = new();

    public int Count { get; private set; }

    public bool Add(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        Node current = _root;
        foreach (char character in word)
        {
            if (!current.Children.TryGetValue(character, out Node? child))
            {
                child = new Node();
                current.Children.Add(character, child);
            }

            current = child;
        }

        if (current.IsWord)
        {
            return false;
        }

        current.IsWord = true;
        Count++;
        return true;
    }

    public bool Contains(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        return FindNode(word)?.IsWord is true;
    }

    public bool ContainsPrefix(string prefix)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        return FindNode(prefix) is not null;
    }

    public string[] FindByPrefix(string prefix, int limit = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentOutOfRangeException.ThrowIfNegative(limit);

        Node? prefixNode = FindNode(prefix);
        if (prefixNode is null || limit == 0)
        {
            return [];
        }

        var results = new List<string>(Math.Min(limit, Count));
        var buffer = new System.Text.StringBuilder(prefix);
        Collect(prefixNode, buffer, results, limit);
        return results.ToArray();
    }

    public bool Remove(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        bool removed = Remove(_root, word, index: 0, out _);
        if (removed)
        {
            Count--;
        }

        return removed;
    }

    private Node? FindNode(string text)
    {
        Node current = _root;
        foreach (char character in text)
        {
            if (!current.Children.TryGetValue(character, out Node? child))
            {
                return null;
            }

            current = child;
        }

        return current;
    }

    private static void Collect(Node node, System.Text.StringBuilder buffer, List<string> results, int limit)
    {
        if (node.IsWord)
        {
            results.Add(buffer.ToString());
            if (results.Count == limit)
            {
                return;
            }
        }

        // SortedDictionary gives deterministic lexical output for teaching and tests.
        foreach ((char character, Node child) in node.Children)
        {
            buffer.Append(character);
            Collect(child, buffer, results, limit);
            buffer.Length--;

            if (results.Count == limit)
            {
                return;
            }
        }
    }

    private static bool Remove(Node node, string word, int index, out bool pruneNode)
    {
        if (index == word.Length)
        {
            if (!node.IsWord)
            {
                pruneNode = false;
                return false;
            }

            node.IsWord = false;
            pruneNode = node.Children.Count == 0;
            return true;
        }

        char character = word[index];
        if (!node.Children.TryGetValue(character, out Node? child))
        {
            pruneNode = false;
            return false;
        }

        bool removed = Remove(child, word, index + 1, out bool pruneChild);
        if (pruneChild)
        {
            node.Children.Remove(character);
        }

        pruneNode = !node.IsWord && node.Children.Count == 0;
        return removed;
    }

    private sealed class Node
    {
        public SortedDictionary<char, Node> Children { get; } = [];
        public bool IsWord { get; set; }
    }
}

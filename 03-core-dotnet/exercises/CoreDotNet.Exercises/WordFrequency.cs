namespace CoreDotNet.Exercises;

public static class WordFrequency
{
    private static readonly char[] Separators = [' ', '\t', '\r', '\n', ',', '.', ';', ':', '!', '?'];

    public static IReadOnlyDictionary<string, int> Count(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        string[] words = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (string word in words)
        {
            // TryGetValue performs one dictionary lookup and makes the update
            // behavior explicit for learners who have not used CollectionsMarshal.
            counts[word] = counts.TryGetValue(word, out int current) ? current + 1 : 1;
        }

        return counts;
    }
}

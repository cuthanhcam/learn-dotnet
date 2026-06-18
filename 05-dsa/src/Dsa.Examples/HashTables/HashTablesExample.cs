namespace Dsa.Examples.HashTables;

public static class HashTablesExample
{
    public static Dictionary<string, int> CountWords(IEnumerable<string> words)
    {
        Dictionary<string, int> counts = new(StringComparer.OrdinalIgnoreCase);

        foreach (string word in words)
        {
            counts[word] = counts.GetValueOrDefault(word) + 1;
        }

        return counts;
    }

    public static int[] TwoSumUnsorted(ReadOnlySpan<int> values, int target)
    {
        Dictionary<int, int> indexByValue = [];

        for (int i = 0; i < values.Length; i++)
        {
            int complement = target - values[i];

            if (indexByValue.TryGetValue(complement, out int complementIndex))
            {
                return [complementIndex, i];
            }

            indexByValue.TryAdd(values[i], i);
        }

        return [];
    }

    public static string FirstUniqueCharacter(string text)
    {
        Dictionary<char, int> counts = [];

        foreach (char character in text)
        {
            counts[character] = counts.GetValueOrDefault(character) + 1;
        }

        foreach (char character in text)
        {
            if (counts[character] == 1)
            {
                return character.ToString();
            }
        }

        return string.Empty;
    }

    public static string[][] GroupAnagrams(IEnumerable<string> words)
    {
        Dictionary<string, List<string>> groups = [];

        foreach (string word in words)
        {
            char[] characters = word.ToCharArray();
            Array.Sort(characters);
            string key = new(characters);

            if (!groups.TryGetValue(key, out List<string>? group))
            {
                group = [];
                groups[key] = group;
            }

            group.Add(word);
        }

        return groups
            .Values
            .Select(group => group.Order(StringComparer.Ordinal).ToArray())
            .OrderBy(group => group[0], StringComparer.Ordinal)
            .ToArray();
    }

    public static void Run()
    {
        Dictionary<string, int> counts = CountWords(["api", "cache", "API", "queue"]);

        Console.WriteLine("Hash tables");
        Console.WriteLine($"api count: {counts["api"]}");
        Console.WriteLine($"Two sum unsorted: {string.Join(", ", TwoSumUnsorted([2, 7, 11, 15], 9))}");
        Console.WriteLine($"First unique in 'swiss': {FirstUniqueCharacter("swiss")}");
    }
}

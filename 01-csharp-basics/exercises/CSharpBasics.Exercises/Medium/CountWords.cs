namespace CSharpBasics.Exercises.Medium;

public static class CountWords
{
    public static Dictionary<string, int> Count(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        string[] words = input.Split(new[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            if (result.TryGetValue(word, out int count))
            {
                result[word] = count + 1;
            }
            else
            {
                result[word] = 1;
            }
        }

        return result;
    }
}

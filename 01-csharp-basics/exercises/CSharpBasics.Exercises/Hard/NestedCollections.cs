namespace CSharpBasics.Exercises.Hard;

public static class NestedCollections
{
    public static List<int> FlattenDistinctSorted(Dictionary<string, List<int>> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        var values = new HashSet<int>();

        foreach (KeyValuePair<string, List<int>> entry in groups)
        {
            if (entry.Value is null)
            {
                continue;
            }

            for (int i = 0; i < entry.Value.Count; i++)
            {
                values.Add(entry.Value[i]);
            }
        }

        var result = values.ToList();
        result.Sort();
        return result;
    }
}

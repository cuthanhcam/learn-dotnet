namespace CSharpBasics.Exercises.Hard;

public static class StudentReport
{
    // Combined exercise for methods, collections, and null-safety.
    public static string BuildReport(Dictionary<string, int?> grades)
    {
        ArgumentNullException.ThrowIfNull(grades);

        if (grades.Count == 0)
        {
            return "No students";
        }

        var parts = new List<string>();
        foreach (KeyValuePair<string, int?> entry in grades)
        {
            string gradeText = entry.Value?.ToString() ?? "N/A";
            parts.Add($"{entry.Key}={gradeText}");
        }

        return string.Join("; ", parts);
    }
}

namespace CSharpBasics.Exercises.Medium;

public static class NullDisplay
{
    // Nullability exercise with a safe fallback.
    public static string GetDisplayName(string? name)
    {
        return string.IsNullOrWhiteSpace(name) ? "Anonymous" : name.Trim();
    }
}

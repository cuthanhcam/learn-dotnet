namespace CSharpBasics.Exercises.Easy;

public static class MethodBasics
{
    // Method exercise with a default value.
    public static string Greet(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? "Hello, world!" : $"Hello, {name}!";
    }
}

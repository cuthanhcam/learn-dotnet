namespace CSharpBasics.Exercises.Easy;

public static class VariableTypes
{
    public static string GetTypeName(object? value)
    {
        return value?.GetType().Name ?? "null";
    }
}

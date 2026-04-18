namespace CSharpBasics.Exercises.Medium;

public static class ReverseString
{
    public static string Reverse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        char[] chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}

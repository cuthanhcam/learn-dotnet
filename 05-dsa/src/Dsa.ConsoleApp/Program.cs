namespace Dsa.ConsoleApp;

public static class Program
{
    public static void Main()
    {
        PrintHeader("Data Structures & Algorithms Demo Runner");
        Console.WriteLine("DSA examples will be grouped by roadmap topic.");
        PrintFooter();
    }

    private static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine(title.ToUpperInvariant().PadLeft((70 + title.Length) / 2));
        Console.WriteLine(new string('=', 70));
    }

    private static void PrintFooter()
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("END OF DEMO".PadLeft(40));
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
    }
}

namespace Dsa.ConsoleApp;

using Dsa.Examples.ArraysStrings;
using Dsa.Examples.Complexity;
using Dsa.Examples.LinkedLists;
using Dsa.Examples.StacksQueues;

public static class Program
{
    public static void Main()
    {
        PrintHeader("Data Structures & Algorithms Demo Runner");

        RunSection("Big-O Notation", ComplexityExample.Run);
        RunSection("Arrays and Strings", ArraysStringsExample.Run);
        RunSection("Linked Lists", LinkedListsExample.Run);
        RunSection("Stacks and Queues", StacksQueuesExample.Run);

        PrintFooter();
    }

    private static void RunSection(string title, Action action)
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 70));
        Console.WriteLine(title.ToUpperInvariant().PadLeft((70 + title.Length) / 2));
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();

        action();
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

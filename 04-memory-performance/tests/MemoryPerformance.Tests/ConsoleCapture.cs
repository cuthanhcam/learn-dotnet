using System.Text;

namespace MemoryPerformance.Tests;

public static class ConsoleCapture
{
    public static string Run(Action action)
    {
        TextWriter originalOut = Console.Out;
        using var writer = new StringWriter(new StringBuilder());

        try
        {
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}

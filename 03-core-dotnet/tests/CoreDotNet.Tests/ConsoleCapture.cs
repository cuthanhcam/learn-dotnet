namespace CoreDotNet.Tests;

public static class ConsoleCapture
{
    public static string Run(Action action)
    {
        var originalOut = Console.Out;
        var writer = new StringWriter();

        Console.SetOut(writer);

        try
        {
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
            writer.Dispose();
        }
    }
}

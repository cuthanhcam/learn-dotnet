using Learning.Patterns.Behavioral.Strategy;

// Explicit registration keeps the learning path discoverable. Default to help, not the output of an
// entire catalog. Unknown arguments return a failing exit code so scripts do not silently run a demo.
if (args.Length == 0 || args.SequenceEqual(new[] { "--help" }) || args.SequenceEqual(new[] { "--list" }))
{
    Console.WriteLine("Usage: dotnet run --project <runner> -- --pattern behavioral.strategy");
    Console.WriteLine("Available: behavioral.strategy");
    return 0;
}
if (args.SequenceEqual(new[] { "--pattern", "behavioral.strategy" }))
{
    StrategyDemo.Run(Console.Out);
    return 0;
}
Console.Error.WriteLine("Unknown pattern or arguments. Use --list to see delivered demos.");
return 2;

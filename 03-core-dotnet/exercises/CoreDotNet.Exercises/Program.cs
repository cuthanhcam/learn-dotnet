using CoreDotNet.Exercises;

Console.WriteLine("Core .NET exercise runner");
Console.WriteLine(new string('=', 40));

IReadOnlyDictionary<string, int> counts = WordFrequency.Count("LINQ and collections; LINQ and events.");
Console.WriteLine(string.Join(", ", counts.Select(pair => $"{pair.Key}:{pair.Value}")));

int[] distinctSquares = QueryExercises.GetDistinctEvenSquares([1, 2, 2, 3, 4, 5]);
Console.WriteLine($"Distinct even squares: {string.Join(", ", distinctSquares)}");

var counter = new ThresholdCounter(threshold: 3);
counter.ThresholdReached += (_, eventArgs) =>
    Console.WriteLine($"Threshold reached at {eventArgs.Value}");

counter.Increment();
counter.Increment();
counter.Increment();

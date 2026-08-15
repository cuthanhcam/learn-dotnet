using AsyncConcurrency.Examples.AsyncBasics;
using AsyncConcurrency.Examples.Channels;
using AsyncConcurrency.Examples.Synchronization;

Console.WriteLine("Async and Concurrency examples");
Console.WriteLine(new string('=', 50));

string[] values = await AsyncBasicsExample.RunConcurrentlyAsync(["task", "await", "io"]);
Console.WriteLine($"Concurrent I/O results: {string.Join(", ", values)}");

var counter = new ThreadSafeCounter();
await Parallel.ForEachAsync(Enumerable.Range(0, 1_000), (_, _) =>
{
    counter.Increment();
    return ValueTask.CompletedTask;
});
Console.WriteLine($"Atomic counter: {counter.Value}");

int[] squares = await ChannelPipelineExample.SquareAsync(Enumerable.Range(1, 5), capacity: 2);
Console.WriteLine($"Bounded channel results: {string.Join(", ", squares)}");

using AsyncConcurrency.Exercises;

Console.WriteLine("Async and Concurrency exercise runner");

int[] doubled = await AsyncMap.MapAsync(
    [1, 2, 3, 4],
    maxConcurrency: 2,
    static async (value, token) =>
    {
        await Task.Delay(TimeSpan.FromMilliseconds(5), token);
        return value * 2;
    });

Console.WriteLine($"Bounded async map: {string.Join(", ", doubled)}");

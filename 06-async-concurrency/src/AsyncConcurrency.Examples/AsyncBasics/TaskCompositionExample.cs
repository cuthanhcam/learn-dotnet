namespace AsyncConcurrency.Examples.AsyncBasics;

public static class TaskCompositionExample
{
    public static async IAsyncEnumerable<T> InCompletionOrder<T>(
        IEnumerable<Task<T>> tasks,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        List<Task<T>> pending = tasks.ToList();

        while (pending.Count > 0)
        {
            Task<T> completed = await Task.WhenAny(pending)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            pending.Remove(completed);
            yield return await completed.ConfigureAwait(false);
        }
    }

    public static async Task<Exception[]> ObserveAllFailuresAsync(IEnumerable<Task> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        Task[] materialized = tasks.ToArray();

        try
        {
            await Task.WhenAll(materialized).ConfigureAwait(false);
            return [];
        }
        catch
        {
            // Await rethrows one failure. Inspect each task after WhenAll has
            // completed to preserve every exception for diagnostics.
            return materialized
                .Where(static task => task.IsFaulted)
                .SelectMany(static task => task.Exception!.InnerExceptions)
                .ToArray();
        }
    }
}

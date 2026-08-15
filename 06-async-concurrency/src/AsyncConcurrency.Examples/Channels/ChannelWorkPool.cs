using System.Threading.Channels;

namespace AsyncConcurrency.Examples.Channels;

/// <summary>
/// Runs asynchronous transformations through a bounded, fixed-size worker pool.
/// Capacity limits queued work, while worker count limits active dependency calls.
/// </summary>
public static class ChannelWorkPool
{
    public static async Task<TResult[]> SelectAsync<TSource, TResult>(
        IEnumerable<TSource> source,
        int workerCount,
        int capacity,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(workerCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        TSource[] items = source.ToArray();
        var results = new TResult[items.Length];
        Channel<WorkItem<TSource>> channel = Channel.CreateBounded<WorkItem<TSource>>(
            new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = true,
                SingleReader = workerCount == 1,
                AllowSynchronousContinuations = false
            });

        // Internal cancellation connects failures in either half of the pipeline. Without it,
        // a producer could remain blocked in WriteAsync after all consumers have faulted.
        using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task producer = ProduceAsync(channel.Writer, items, pipelineCancellation.Token);
        Task[] workers = Enumerable.Range(0, workerCount)
            .Select(_ => ConsumeAsync(channel.Reader, results, selector, pipelineCancellation))
            .ToArray();
        Task allWorkers = Task.WhenAll(workers);

        try
        {
            await Task.WhenAll(producer, allWorkers).ConfigureAwait(false);
            return results;
        }
        catch
        {
            // Prefer the worker's domain failure over the cancellation it caused in the
            // producer. This keeps the useful root cause at the public operation boundary.
            if (allWorkers.IsFaulted)
            {
                await allWorkers.ConfigureAwait(false);
            }

            await producer.ConfigureAwait(false);
            throw;
        }
        finally
        {
            // Wakes the opposite side promptly when one task fails or the caller cancels.
            await pipelineCancellation.CancelAsync().ConfigureAwait(false);
        }
    }

    private static async Task ProduceAsync<TSource>(
        ChannelWriter<WorkItem<TSource>> writer,
        IReadOnlyList<TSource> items,
        CancellationToken cancellationToken)
    {
        Exception? completionError = null;
        try
        {
            for (int index = 0; index < items.Count; index++)
            {
                await writer.WriteAsync(new WorkItem<TSource>(index, items[index]), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            completionError = exception;
            throw;
        }
        finally
        {
            // Readers terminate after buffered items are drained, or observe the producer error.
            writer.TryComplete(completionError);
        }
    }

    private static async Task ConsumeAsync<TSource, TResult>(
        ChannelReader<WorkItem<TSource>> reader,
        TResult[] results,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationTokenSource pipelineCancellation)
    {
        try
        {
            await foreach (WorkItem<TSource> item in reader
                .ReadAllAsync(pipelineCancellation.Token)
                .ConfigureAwait(false))
            {
                // Each index is written exactly once, so workers do not need a lock around the
                // result array. Indexing also preserves input order despite out-of-order finishes.
                results[item.Index] = await selector(item.Value, pipelineCancellation.Token)
                    .ConfigureAwait(false);
            }
        }
        catch
        {
            await pipelineCancellation.CancelAsync().ConfigureAwait(false);
            throw;
        }
    }

    private readonly record struct WorkItem<T>(int Index, T Value);
}

using System.Runtime.CompilerServices;

namespace AsyncConcurrency.Examples.AsyncBasics;

public static class AsyncStreamOperators
{
    /// <summary>
    /// Groups a streaming source into bounded arrays without materializing the full sequence.
    /// The final array can contain fewer than <paramref name="batchSize"/> items.
    /// </summary>
    public static async IAsyncEnumerable<T[]> Buffer<T>(
        IAsyncEnumerable<T> source,
        int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);

        var buffer = new List<T>(batchSize);
        await foreach (T item in source
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false))
        {
            buffer.Add(item);
            if (buffer.Count != batchSize)
            {
                continue;
            }

            // Yield a new array because the mutable list is immediately cleared and reused.
            // Returning the list itself would expose later mutations to earlier consumers.
            yield return buffer.ToArray();
            buffer.Clear();
        }

        if (buffer.Count > 0)
        {
            yield return buffer.ToArray();
        }
    }

    /// <summary>
    /// Applies one asynchronous transformation at a time and preserves streaming order.
    /// Use a bounded worker pool instead when independent transforms should overlap.
    /// </summary>
    public static async IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(
        IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TResult>> selector,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        await foreach (TSource item in source
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false))
        {
            yield return await selector(item, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Materializes at most <paramref name="maximumCount"/> elements.
    /// The explicit guard prevents an accidentally infinite or unexpectedly large stream
    /// from growing memory without a caller-defined bound.
    /// </summary>
    public static async Task<IReadOnlyList<T>> ToBoundedListAsync<T>(
        IAsyncEnumerable<T> source,
        int maximumCount,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(maximumCount);

        var result = new List<T>(Math.Min(maximumCount, 256));
        await foreach (T item in source
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false))
        {
            if (result.Count == maximumCount)
            {
                throw new InvalidOperationException(
                    $"The stream exceeded the configured limit of {maximumCount} elements.");
            }

            result.Add(item);
        }

        return result;
    }
}

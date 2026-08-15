using System.Threading.Channels;

namespace AsyncConcurrency.Examples.Channels;

public static class ChannelPipelineExample
{
    public static async Task<int[]> SquareAsync(
        IEnumerable<int> values,
        int capacity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        Channel<int> channel = Channel.CreateBounded<int>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = true
        });

        Task producer = ProduceAsync(channel.Writer, values, cancellationToken);
        Task<int[]> consumer = ConsumeAsync(channel.Reader, cancellationToken);

        await producer.ConfigureAwait(false);
        return await consumer.ConfigureAwait(false);
    }

    private static async Task ProduceAsync(
        ChannelWriter<int> writer,
        IEnumerable<int> values,
        CancellationToken cancellationToken)
    {
        Exception? failure = null;
        try
        {
            foreach (int value in values)
            {
                await writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            failure = exception;
            throw;
        }
        finally
        {
            // Completing the writer is essential: otherwise the reader can wait
            // forever after the final item. The failure is propagated to readers.
            writer.TryComplete(failure);
        }
    }

    private static async Task<int[]> ConsumeAsync(
        ChannelReader<int> reader,
        CancellationToken cancellationToken)
    {
        var results = new List<int>();
        await foreach (int value in reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            results.Add(checked(value * value));
        }

        return results.ToArray();
    }
}

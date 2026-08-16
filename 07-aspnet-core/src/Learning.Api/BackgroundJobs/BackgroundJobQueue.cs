using System.Threading.Channels;

namespace Learning.Api.BackgroundJobs;

public sealed class BackgroundJobQueue
{
    public const int Capacity = 2;
    private readonly Channel<BackgroundJob> _channel = Channel.CreateBounded<BackgroundJob>(
        new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

    public bool TryEnqueue(BackgroundJob job) => _channel.Writer.TryWrite(job);

    public IAsyncEnumerable<BackgroundJob> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);

    public void Complete() => _channel.Writer.TryComplete();
}

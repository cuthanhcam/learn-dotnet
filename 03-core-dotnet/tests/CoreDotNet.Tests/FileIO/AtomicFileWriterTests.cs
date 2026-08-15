using CoreDotNet.Examples.FileIO;

namespace CoreDotNet.Tests.FileIO;

public sealed class AtomicFileWriterTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"learn-dotnet-atomic-write-{Guid.NewGuid():N}");

    public AtomicFileWriterTests() => Directory.CreateDirectory(_directory);

    [Fact]
    public async Task WriteTextAsync_ReplacesTargetWithCompleteUtf8Content()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "old");

        await AtomicFileWriter.WriteTextAsync(path, "{\"message\":\"Hello, .NET 👋\"}");

        Assert.Equal("{\"message\":\"Hello, .NET 👋\"}", await File.ReadAllTextAsync(path));
        Assert.Empty(Directory.EnumerateFiles(_directory, ".*.tmp"));
    }

    [Fact]
    public async Task WriteAsync_FailurePreservesOriginalAndDeletesTemporaryFile()
    {
        string path = Path.Combine(_directory, "important.txt");
        await File.WriteAllTextAsync(path, "original");

        await Assert.ThrowsAsync<IOException>(() => AtomicFileWriter.WriteAsync(
            path,
            async (stream, token) =>
            {
                await stream.WriteAsync("partial"u8.ToArray(), token);
                throw new IOException("simulated failure");
            }));

        Assert.Equal("original", await File.ReadAllTextAsync(path));
        Assert.Empty(Directory.EnumerateFiles(_directory, ".*.tmp"));
    }

    [Fact]
    public async Task WriteAsync_PreCanceledTokenPreservesOriginal()
    {
        string path = Path.Combine(_directory, "cancelled.txt");
        await File.WriteAllTextAsync(path, "original");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            AtomicFileWriter.WriteTextAsync(path, "replacement", cancellation.Token));

        Assert.Equal("original", await File.ReadAllTextAsync(path));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}

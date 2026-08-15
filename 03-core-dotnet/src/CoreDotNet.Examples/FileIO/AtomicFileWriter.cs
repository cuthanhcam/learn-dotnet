using System.Text;

namespace CoreDotNet.Examples.FileIO;

public static class AtomicFileWriter
{
    public static Task WriteTextAsync(
        string path,
        string content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        return WriteAsync(
            path,
            async (stream, token) =>
            {
                await using var writer = new StreamWriter(
                    stream,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                    bufferSize: 4096,
                    leaveOpen: true);
                await writer.WriteAsync(content.AsMemory(), token).ConfigureAwait(false);
                await writer.FlushAsync(token).ConfigureAwait(false);
            },
            cancellationToken);
    }

    /// <summary>
    /// Writes a complete temporary file beside the target, then replaces the target by rename.
    /// </summary>
    public static async Task WriteAsync(
        string path,
        Func<Stream, CancellationToken, Task> writeContent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(writeContent);

        string targetPath = Path.GetFullPath(path);
        string directory = Path.GetDirectoryName(targetPath)!;
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Target directory does not exist: {directory}");
        }

        string temporaryPath = Path.Combine(
            directory,
            $".{Path.GetFileName(targetPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                options: FileOptions.Asynchronous))
            {
                await writeContent(stream, cancellationToken).ConfigureAwait(false);

                // Flush managed buffers before closing. Durable persistence across sudden power
                // loss has stronger platform/filesystem requirements than atomic visibility.
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Keeping the temp file in the same directory avoids a cross-volume move. The rename
            // prevents readers from observing a partially written target on supported filesystems.
            File.Move(temporaryPath, targetPath, overwrite: true);
        }
        finally
        {
            // Success moves the temp path away; failure or cancellation leaves it for cleanup here.
            File.Delete(temporaryPath);
        }
    }
}

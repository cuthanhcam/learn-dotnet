using CoreDotNet.Examples.FileIO;

namespace CoreDotNet.Tests.FileIO;

[Collection("Console")]
public class FileIOExampleTests
{
    [Fact]
    public void Run_Prints_File_And_Directory_Operations()
    {
        string output = ConsoleCapture.Run(FileIOExample.Run);

        Assert.Contains("File I/O Examples", output);
        Assert.Contains("Temp file path:", output);
        Assert.Contains("Atomic write completed:", output);
        Assert.Contains("Deleted directory:", output);
    }
}

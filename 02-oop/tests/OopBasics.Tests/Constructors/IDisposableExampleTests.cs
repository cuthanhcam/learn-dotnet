using OopBasics.Examples.Constructors;
using Xunit;

namespace OopBasics.Tests.Constructors;

public class IDisposableExampleTests
{
    [Fact]
    public void FileResource_ImplementsIDisposable()
    {
        using var resource = new FileResource();
        resource.Use();
        // Dispose is called automatically at the end of the using block.
    }
}

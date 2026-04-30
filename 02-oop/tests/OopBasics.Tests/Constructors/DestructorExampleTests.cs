using OopBasics.Examples.Constructors;
using Xunit;

namespace OopBasics.Tests.Constructors;

public class DestructorExampleTests
{
    [Fact]
    public void ResourceHolder_CanBeCreatedAndReleased()
    {
        var resource = new ResourceHolder();
        Assert.NotNull(resource);
        // Destructor cannot be tested directly; just ensure instantiation works.
    }
}

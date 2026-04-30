using OopBasics.Examples.AccessModifiers;
using Xunit;

namespace OopBasics.Tests.AccessModifiers;

public class InheritanceAccessExampleTests
{
    [Fact]
    public void Child_CanAccessProtectedMethod()
    {
        var child = new Child();
        // Should not throw, protected method is accessible in derived class
        child.TestAccess();
    }
}

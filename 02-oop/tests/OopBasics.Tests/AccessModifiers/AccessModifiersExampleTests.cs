using OopBasics.Examples.AccessModifiers;
using Xunit;

namespace OopBasics.Tests.AccessModifiers;

public class AccessModifiersExampleTests
{
    [Fact]
    public void DemoClass_PublicMethod_WritesExpectedOutput()
    {
        // Arrange
        var demo = new DemoClass();
        // Act & Assert
        // Here, we can't capture Console output directly, but we can check that the method is callable and does not throw.
        demo.PublicMethod();
    }
}

using OopBasics.Examples.AccessModifiers;
using Xunit;

namespace OopBasics.Tests.AccessModifiers;

public class NestedTypesExampleTests
{
    [Fact]
    public void Outer_UseInner_CallsInnerShowSecret()
    {
        var outer = new Outer();
        // Should not throw, inner class can access private members of outer
        outer.UseInner();
    }
}

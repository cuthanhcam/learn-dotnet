using OopBasics.Examples.AccessModifiers;
using Xunit;

namespace OopBasics.Tests.AccessModifiers;

public class ProtectedInternalExampleTests
{
    [Fact]
    public void AdvancedDerived_CanAccessProtectedInternalAndPrivateProtected()
    {
        var derived = new AdvancedDerived();
        derived.Test();
    }
}

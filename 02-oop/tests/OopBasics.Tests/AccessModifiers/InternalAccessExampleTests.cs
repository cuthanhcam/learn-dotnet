using OopBasics.Examples.AccessModifiers;
using Xunit;

namespace OopBasics.Tests.AccessModifiers;

public class InternalAccessExampleTests
{
    [Fact]
    public void InternalAccessExample_Run_DoesNotThrow()
    {
        // Just ensure the Run method executes, which internally uses InternalService
        InternalAccessExample.Run();
    }
}

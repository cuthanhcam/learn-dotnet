using OopBasics.Examples.Inheritance;
using Xunit;

namespace OopBasics.Tests.Inheritance;

public class SealedAndOverrideExampleTests
{
    [Fact]
    public void PaymentProcessor_Process_OverridesBase()
    {
        BaseProcessor processor = new PaymentProcessor();
        processor.Process(); // Should call the override, not base
    }
}

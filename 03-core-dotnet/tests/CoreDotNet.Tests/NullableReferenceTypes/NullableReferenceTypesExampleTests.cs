using CoreDotNet.Examples.NullableReferenceTypes;

namespace CoreDotNet.Tests.NullableReferenceTypes;

[Collection("Console")]
public class NullableReferenceTypesExampleTests
{
    [Fact]
    public void Person_GetDisplayInfo_Returns_Projected_Model()
    {
        var person = new Person { Name = "Alice" };

        var info = person.GetDisplayInfo();

        Assert.NotNull(info);
        Assert.Equal("Alice", info!.Name);
    }

    [Fact]
    public void Run_Prints_Nullability_Examples()
    {
        string output = ConsoleCapture.Run(NullableReferenceTypesExample.Run);

        Assert.Contains("Nullable Reference Types Examples", output);
        Assert.Contains("Fallback text: Guest learner", output);
        Assert.Contains("Nullable int HasValue: False", output);
    }
}

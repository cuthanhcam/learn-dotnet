using CSharpBasics.Examples.Variables;
using Xunit;

namespace CSharpBasics.Tests;

public class VariablesTests
{
    [Fact]
    public void GetPrimitiveValues_ReturnsValidSnapshot()
    {
        var snapshot = VariablesExamples.GetPrimitiveValues();
        Assert.NotEqual(default, snapshot.Id);
    }
}

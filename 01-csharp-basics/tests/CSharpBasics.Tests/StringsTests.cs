using CSharpBasics.Examples.Strings;
using Xunit;

namespace CSharpBasics.Tests;

public class StringsTests
{
    [Fact]
    public void AreEqualIgnoreCase_WithDifferentCases_ReturnsTrue()
    {
        bool result = StringBasicsExample.AreEqualIgnoreCase("DOTNET","dotnet");
        Assert.True(result);
    }
}

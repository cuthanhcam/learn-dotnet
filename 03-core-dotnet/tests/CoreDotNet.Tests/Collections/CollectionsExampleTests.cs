using CoreDotNet.Examples.Collections;

namespace CoreDotNet.Tests.Collections;

[Collection("Console")]
public class CollectionsExampleTests
{
    [Fact]
    public void Run_Prints_Collection_Basics()
    {
        string output = ConsoleCapture.Run(CollectionsExample.Run);

        Assert.Contains("Collections Examples", output);
        Assert.Contains("Initial list: apple, banana, cherry", output);
        Assert.Contains("Count: 4, First: apple, Last: date", output);
        Assert.Contains("Upper-case snapshot: APPLE, BANANA, CHERRY, DATE", output);
    }
}

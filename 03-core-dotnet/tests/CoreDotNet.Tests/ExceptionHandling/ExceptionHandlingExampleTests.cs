using CoreDotNet.Examples.ExceptionHandling;

namespace CoreDotNet.Tests.ExceptionHandling;

[Collection("Console")]
public class ExceptionHandlingExampleTests
{
    [Fact]
    public void InvalidAgeException_Stores_The_Invalid_Age()
    {
        var exception = new InvalidAgeException(-5);

        Assert.Equal(-5, exception.InvalidAge);
        Assert.Contains("Age must be between 0 and 150, got -5", exception.Message);
    }

    [Fact]
    public void BusinessException_Preserves_The_Inner_Exception()
    {
        var inner = new InvalidOperationException("Payment declined");
        var exception = new BusinessException("Order processing failed", inner);

        Assert.Same(inner, exception.InnerException);
        Assert.Equal("Order processing failed", exception.Message);
    }

    [Fact]
    public void DisposableResource_Disposes_Without_Throwing()
    {
        using var resource = new DisposableResource();

        resource.DoWork();

        Assert.True(true);
    }

    [Fact]
    public void Run_Prints_Retry_And_Cleanup_Output()
    {
        string output = ConsoleCapture.Run(ExceptionHandlingExample.Run);

        Assert.Contains("Exception Handling Examples", output);
        Assert.Contains("Retry with exponential backoff:", output);
        Assert.Contains("Operation succeeded!", output);
        Assert.Contains("Resource cleaned up", output);
    }
}

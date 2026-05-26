using CoreDotNet.Examples.LINQ;

namespace CoreDotNet.Tests.LINQ;

[Collection("Console")]
public class LINQExampleTests
{
    [Fact]
    public void Run_Prints_LINQ_Sections()
    {
        string output = ConsoleCapture.Run(LINQExample.Run);

        Assert.Contains("LINQ Examples", output);
        Assert.Contains("Query syntax result:", output);
        Assert.Contains("Deferred query includes newly added 6:", output);
        Assert.Contains("Batches of 2:", output);
    }

    [Fact]
    public void Product_Holds_Values()
    {
        var product = new Product { Id = 10, Name = "Notebook", Price = 8m, Category = "Books" };

        Assert.Equal(10, product.Id);
        Assert.Equal("Notebook", product.Name);
        Assert.Equal(8m, product.Price);
        Assert.Equal("Books", product.Category);
    }
}

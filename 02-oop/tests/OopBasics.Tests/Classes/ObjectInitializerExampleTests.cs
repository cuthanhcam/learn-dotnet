using OopBasics.Examples.Classes;
using Xunit;

namespace OopBasics.Tests.Classes;

public class ObjectInitializerExampleTests
{
    [Fact]
    public void Book_ObjectInitializer_SetsProperties()
    {
        var book = new Book
        {
            Title = "C# in Depth",
            Author = "Jon Skeet",
            Pages = 900
        };
        Assert.Equal("C# in Depth", book.Title);
        Assert.Equal("Jon Skeet", book.Author);
        Assert.Equal(900, book.Pages);
    }
}

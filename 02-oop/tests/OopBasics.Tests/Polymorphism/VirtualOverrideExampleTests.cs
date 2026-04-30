using OopBasics.Examples.Polymorphism;
using Xunit;

namespace OopBasics.Tests.Polymorphism;

public class VirtualOverrideExampleTests
{
    [Fact]
    public void Circle_And_Rectangle_GetArea_Correct()
    {
        Shape circle = new Circle(5);
        Shape rectangle = new Rectangle(4, 6);
        Assert.Equal(Math.PI * 25, circle.GetArea(), 3);
        Assert.Equal(24, rectangle.GetArea(), 3);
    }
}

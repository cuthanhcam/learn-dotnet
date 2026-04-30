using OopBasics.Examples.Inheritance;
using Xunit;

namespace OopBasics.Tests.Inheritance;

public class InheritanceExampleTests
{
    [Fact]
    public void Dog_And_Cat_Properties_AndSpeak()
    {
        var dog = new Dog("Buddy", 3);
        var cat = new Cat("Whiskers", 2);
        Assert.Equal("Buddy", dog.Name);
        Assert.Equal(3, dog.Age);
        Assert.Equal("Whiskers", cat.Name);
        Assert.Equal(2, cat.Age);
        // Speak() just writes to console, so we only check that it does not throw
        dog.Speak();
        cat.Speak();
    }

    [Fact]
    public void Animal_Constructor_ThrowsOnInvalidInput()
    {
        Assert.Throws<ArgumentException>(() => new Dog("", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cat("Kitty", -1));
    }
}

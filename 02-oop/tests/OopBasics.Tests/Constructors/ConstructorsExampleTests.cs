using OopBasics.Examples.Constructors;
using Xunit;

namespace OopBasics.Tests.Constructors;

public class ConstructorsExampleTests
{
    [Fact]
    public void User_ConstructorOverloads_InitializeCorrectly()
    {
        var user1 = new User("Alice");
        Assert.Equal("Alice", user1.Name);
        Assert.Equal(0, user1.Age);
        var user2 = new User("Bob", 25);
        Assert.Equal("Bob", user2.Name);
        Assert.Equal(25, user2.Age);
    }
}

using OopBasics.Examples.Classes;
using Xunit;

namespace OopBasics.Tests.Classes;

public class ImmutableObjectExampleTests
{
    [Fact]
    public void User_WithCreatesModifiedCopy_AndEquality()
    {
        var user1 = new User("Alice", 25);
        var user2 = user1 with { Age = 26 };
        Assert.Equal("Alice", user1.Name);
        Assert.Equal(25, user1.Age);
        Assert.Equal("Alice", user2.Name);
        Assert.Equal(26, user2.Age);
        Assert.NotEqual(user1, user2);
    }
}

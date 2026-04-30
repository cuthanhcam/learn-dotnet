using OopBasics.Examples.StaticMembers;
using Xunit;

namespace OopBasics.Tests.StaticMembers;

public class StaticMembersExampleTests
{
    [Fact]
    public void User_TotalUsers_TracksInstanceCount()
    {
        // Reset static field for test isolation (not ideal, but for demonstration)
        var initial = User.TotalUsers;
        var user1 = new User("Alice");
        var user2 = new User("Bob");
        Assert.True(User.TotalUsers >= initial + 2);
    }

    [Fact]
    public void MathHelper_Add_And_Multiply_WorkCorrectly()
    {
        Assert.Equal(8, MathHelper.Add(5, 3));
        Assert.Equal(15, MathHelper.Multiply(5, 3));
    }
}

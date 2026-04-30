using OopBasics.Examples.Classes;
using Xunit;

namespace OopBasics.Tests.Classes;

public class ValueObjectExampleTests
{
    [Fact]
    public void Email_ValidAndInvalid()
    {
        var email = new Email("user@example.com");
        Assert.Equal("user@example.com", email.Value);
        Assert.Throws<ArgumentException>(() => new Email("invalid-email"));
    }
}

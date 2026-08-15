using OopBasics.Examples.Classes;
namespace OopBasics.Tests.Classes;

public class ValueObjectExampleTests
{
    [Fact]
    public void Email_ValidAndInvalid()
    {
        var email = new Email("user@Example.COM");
        Assert.Equal("user@example.com", email.Value);
        Assert.Throws<ArgumentException>(() => new Email("invalid-email"));
    }

    [Fact]
    public void Email_EqualityAndHashCodeFollowNormalizedDomainValue()
    {
        var first = new Email("user@Example.COM");
        var equivalent = new Email("user@example.com");
        var differentLocalCase = new Email("USER@example.com");

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, differentLocalCase);

        HashSet<Email> addresses = [first, equivalent, differentLocalCase];
        Assert.Equal(2, addresses.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("missing-at.example.com")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@@example.com")]
    [InlineData("user @example.com")]
    public void Email_RejectsValuesOutsideItsDocumentedContract(string value)
    {
        Assert.False(Email.TryCreate(value, out Email? email, out string? error));
        Assert.Null(email);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }
}

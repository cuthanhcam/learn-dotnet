using Learning.Auth.Domain.Users;

namespace Learning.Auth.UnitTests;

public sealed class EmailAddressTests
{
    [Fact]
    public void Create_NormalizesLookupValueWithoutChangingDisplayValue()
    {
        EmailAddress address = EmailAddress.Create("  Learner@Example.com ");

        Assert.Equal("Learner@Example.com", address.Value);
        Assert.Equal("LEARNER@EXAMPLE.COM", address.NormalizedValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("missing-domain@")]
    [InlineData("missing-at.example")]
    [InlineData("learner@example.com\r\nForged: value")]
    public void Create_InvalidValueThrows(string value) =>
        Assert.Throws<ArgumentException>(() => EmailAddress.Create(value));
}

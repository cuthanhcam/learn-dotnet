using Learning.Auth.Application.Abstractions;
using Learning.Auth.Infrastructure.Identity;

namespace Learning.Auth.UnitTests;

public sealed class PasswordHashServiceTests
{
    private readonly AspNetCorePasswordHashService _passwords = new();

    [Fact]
    public void Hash_SamePasswordTwiceUsesDifferentRandomSalts()
    {
        string first = _passwords.Hash("correct horse battery staple");
        string second = _passwords.Hash("correct horse battery staple");

        Assert.NotEqual(first, second);
        Assert.Equal(PasswordVerification.Succeeded, _passwords.Verify(first, "correct horse battery staple"));
        Assert.Equal(PasswordVerification.Succeeded, _passwords.Verify(second, "correct horse battery staple"));
    }

    [Fact]
    public void Verify_WrongPasswordFails()
    {
        string hash = _passwords.Hash("correct horse battery staple");

        Assert.Equal(PasswordVerification.Failed, _passwords.Verify(hash, "this password is wrong"));
    }
}

using Learning.Auth.Application.Identity;
using Learning.Auth.Infrastructure.Identity;

namespace Learning.Auth.UnitTests;

public sealed class IdentityServicesTests
{
    private readonly InMemoryUserAccountRepository _accounts = new();
    private readonly AspNetCorePasswordHashService _passwords = new();

    [Fact]
    public async Task Register_ConcurrentEquivalentEmailsCreatesExactlyOneAccount()
    {
        var registration = new RegistrationService(_accounts, _passwords, TimeProvider.System);

        RegistrationResult[] results = await Task.WhenAll(
            registration.RegisterAsync("Learner@example.com", "correct horse battery staple"),
            registration.RegisterAsync("learner@EXAMPLE.com", "correct horse battery staple"));

        Assert.Single(results, result => result.Status == RegistrationStatus.Created);
        Assert.Single(results, result => result.Status == RegistrationStatus.EmailUnavailable);
    }

    [Fact]
    public async Task SignIn_ValidCredentialsReturnsAccount()
    {
        var registration = new RegistrationService(_accounts, _passwords, TimeProvider.System);
        var signIn = new CredentialSignInService(_accounts, _passwords);
        await registration.RegisterAsync("learner@example.com", "correct horse battery staple");

        SignInResult result = await signIn.VerifyAsync(
            "LEARNER@example.com",
            "correct horse battery staple");

        Assert.Equal(SignInStatus.Succeeded, result.Status);
        Assert.Equal("learner@example.com", result.Account!.Email.Value);
    }

    [Theory]
    [InlineData("unknown@example.com", "correct horse battery staple")]
    [InlineData("not-an-email", "correct horse battery staple")]
    public async Task SignIn_UnknownOrMalformedIdentityUsesGenericFailure(string email, string password)
    {
        var signIn = new CredentialSignInService(_accounts, _passwords);

        SignInResult result = await signIn.VerifyAsync(email, password);

        Assert.Equal(SignInStatus.InvalidCredentials, result.Status);
        Assert.Null(result.Account);
    }
}

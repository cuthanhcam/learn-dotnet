using Learning.Auth.Application.Identity;
using Learning.Auth.Domain.Users;
using Learning.Auth.Infrastructure.Identity;

namespace Learning.Auth.UnitTests;

public sealed class IdentityServicesTests
{
    private static readonly SignInSecurityOptions SecurityOptions = new();
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
        var signIn = new CredentialSignInService(_accounts, _passwords, TimeProvider.System, SecurityOptions);
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
        var signIn = new CredentialSignInService(_accounts, _passwords, TimeProvider.System, SecurityOptions);

        SignInResult result = await signIn.VerifyAsync(email, password);

        Assert.Equal(SignInStatus.InvalidCredentials, result.Status);
        Assert.Null(result.Account);
    }

    [Fact]
    public async Task SignIn_RepeatedFailuresLockAccountUntilConfiguredTimePasses()
    {
        var time = new ManualTimeProvider(new DateTimeOffset(2026, 9, 2, 8, 0, 0, TimeSpan.Zero));
        var registration = new RegistrationService(_accounts, _passwords, time);
        var signIn = new CredentialSignInService(_accounts, _passwords, time, SecurityOptions);
        await registration.RegisterAsync("locked@example.com", "correct horse battery staple");

        for (int attempt = 0; attempt < SecurityOptions.MaxFailedAttempts; attempt++)
        {
            SignInResult failure = await signIn.VerifyAsync("locked@example.com", "wrong password");
            Assert.Equal(SignInStatus.InvalidCredentials, failure.Status);
        }

        SignInResult duringLockout = await signIn.VerifyAsync(
            "locked@example.com", "correct horse battery staple");
        Assert.Equal(SignInStatus.AccountUnavailable, duringLockout.Status);

        time.Advance(SecurityOptions.LockoutDuration.Add(TimeSpan.FromSeconds(1)));
        SignInResult recovered = await signIn.VerifyAsync(
            "locked@example.com", "correct horse battery staple");

        Assert.Equal(SignInStatus.Succeeded, recovered.Status);
        Assert.Equal(0, recovered.Account!.FailedSignInCount);
        Assert.Null(recovered.Account.LockoutEnd);
    }

    [Fact]
    public async Task SignIn_ConcurrentFailuresCannotBypassAtomicLockoutTransition()
    {
        var time = new ManualTimeProvider(new DateTimeOffset(2026, 9, 2, 8, 0, 0, TimeSpan.Zero));
        var registration = new RegistrationService(_accounts, _passwords, time);
        var signIn = new CredentialSignInService(_accounts, _passwords, time, SecurityOptions);
        await registration.RegisterAsync("parallel@example.com", "correct horse battery staple");

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ =>
            signIn.VerifyAsync("parallel@example.com", "wrong password")));

        string normalizedEmail = EmailAddress.Create("parallel@example.com").NormalizedValue;
        UserAccount account = (await _accounts.FindByNormalizedEmailAsync(
            normalizedEmail, CancellationToken.None))!;
        Assert.Equal(SecurityOptions.MaxFailedAttempts, account.FailedSignInCount);
        Assert.True(account.IsLockedOut(time.GetUtcNow()));
    }
}

internal sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    private DateTimeOffset _utcNow = utcNow;

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void Advance(TimeSpan duration) => _utcNow = _utcNow.Add(duration);
}

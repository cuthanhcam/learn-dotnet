using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;

namespace Learning.Auth.Application.Identity;

public enum SignInStatus
{
    Succeeded,
    InvalidCredentials,
    AccountUnavailable
}

public sealed record SignInResult(SignInStatus Status, UserAccount? Account = null);

public sealed class CredentialSignInService(
    IUserAccountRepository accounts,
    IPasswordHashService passwords,
    TimeProvider timeProvider,
    SignInSecurityOptions securityOptions,
    ISecurityEventSink securityEvents)
{
    public async Task<SignInResult> VerifyAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        EmailAddress address;
        try
        {
            address = EmailAddress.Create(email);
        }
        catch (ArgumentException)
        {
            // Malformed and unknown identities share the same public outcome.
            passwords.VerifyUnknownAccount(password);
            await WriteEventAsync(SecurityEventType.SignInRejected, null, cancellationToken).ConfigureAwait(false);
            return new SignInResult(SignInStatus.InvalidCredentials);
        }

        UserAccount? account = await accounts
            .FindByNormalizedEmailAsync(address.NormalizedValue, cancellationToken)
            .ConfigureAwait(false);

        if (account is null)
        {
            passwords.VerifyUnknownAccount(password);
            await WriteEventAsync(SecurityEventType.SignInRejected, null, cancellationToken).ConfigureAwait(false);
            return new SignInResult(SignInStatus.InvalidCredentials);
        }

        PasswordVerification verification = passwords.Verify(account.PasswordHash, password);
        if (verification == PasswordVerification.Failed)
        {
            bool wasLockedOut = account.IsLockedOut(timeProvider.GetUtcNow());
            account.RecordFailedSignIn(timeProvider.GetUtcNow(), securityOptions.MaxFailedAttempts,
                securityOptions.LockoutDuration);
            await accounts.UpdateAsync(account, cancellationToken).ConfigureAwait(false);
            await WriteEventAsync(SecurityEventType.SignInRejected, account.Id, cancellationToken)
                .ConfigureAwait(false);
            if (!wasLockedOut && account.IsLockedOut(timeProvider.GetUtcNow()))
            {
                await WriteEventAsync(SecurityEventType.AccountLockoutStarted, account.Id, cancellationToken)
                    .ConfigureAwait(false);
            }
            return new SignInResult(SignInStatus.InvalidCredentials);
        }

        DateTimeOffset now = timeProvider.GetUtcNow();
        if (account.Status != AccountStatus.Active || account.IsLockedOut(now))
        {
            await WriteEventAsync(SecurityEventType.SignInRejected, account.Id, cancellationToken)
                .ConfigureAwait(false);
            return new SignInResult(SignInStatus.AccountUnavailable);
        }

        // A correct credential outside an active lockout begins a clean failure sequence. Persist this
        // together with a potential password rehash in one repository update.
        account.RecordSuccessfulSignIn();
        if (verification == PasswordVerification.SucceededRehashNeeded)
            account.ReplacePasswordHash(passwords.Hash(password));
        await accounts.UpdateAsync(account, cancellationToken).ConfigureAwait(false);
        await WriteEventAsync(SecurityEventType.SignInSucceeded, account.Id, cancellationToken)
            .ConfigureAwait(false);

        return new SignInResult(SignInStatus.Succeeded, account);
    }

    private ValueTask WriteEventAsync(SecurityEventType type, Guid? subjectId,
        CancellationToken cancellationToken) => securityEvents.WriteAsync(
            new SecurityEvent(type, timeProvider.GetUtcNow(), subjectId), cancellationToken);
}

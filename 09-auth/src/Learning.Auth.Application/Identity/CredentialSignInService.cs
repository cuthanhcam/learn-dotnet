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
    IPasswordHashService passwords)
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
            return new SignInResult(SignInStatus.InvalidCredentials);
        }

        UserAccount? account = await accounts
            .FindByNormalizedEmailAsync(address.NormalizedValue, cancellationToken)
            .ConfigureAwait(false);

        if (account is null)
        {
            passwords.VerifyUnknownAccount(password);
            return new SignInResult(SignInStatus.InvalidCredentials);
        }

        PasswordVerification verification = passwords.Verify(account.PasswordHash, password);
        if (verification == PasswordVerification.Failed)
        {
            return new SignInResult(SignInStatus.InvalidCredentials);
        }

        if (account.Status != AccountStatus.Active)
        {
            return new SignInResult(SignInStatus.AccountUnavailable);
        }

        if (verification == PasswordVerification.SucceededRehashNeeded)
        {
            account.ReplacePasswordHash(passwords.Hash(password));
            await accounts.UpdateAsync(account, cancellationToken).ConfigureAwait(false);
        }

        return new SignInResult(SignInStatus.Succeeded, account);
    }
}

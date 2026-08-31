using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;

namespace Learning.Auth.Application.Identity;

public enum RegistrationStatus
{
    Created,
    EmailUnavailable
}

public sealed record RegistrationResult(RegistrationStatus Status, Guid? UserId = null);

public sealed class RegistrationService(
    IUserAccountRepository accounts,
    IPasswordHashService passwords,
    TimeProvider timeProvider)
{
    public async Task<RegistrationResult> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        EmailAddress address = EmailAddress.Create(email);
        ValidatePassword(password);

        // Hash before the atomic insert. Concurrent duplicate attempts still perform comparable
        // expensive work and the repository remains the final uniqueness authority.
        string hash = passwords.Hash(password);
        UserAccount account = UserAccount.Register(
            Guid.NewGuid(),
            address,
            hash,
            timeProvider.GetUtcNow());

        bool added = await accounts.TryAddAsync(account, cancellationToken).ConfigureAwait(false);
        return added
            ? new RegistrationResult(RegistrationStatus.Created, account.Id)
            : new RegistrationResult(RegistrationStatus.EmailUnavailable);
    }

    private static void ValidatePassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        if (password.Length is < 12 or > 128)
        {
            throw new ArgumentException("Password length must be between 12 and 128 characters.", nameof(password));
        }
    }
}

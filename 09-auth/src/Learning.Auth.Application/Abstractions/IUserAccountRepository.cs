using Learning.Auth.Domain.Users;

namespace Learning.Auth.Application.Abstractions;

public interface IUserAccountRepository
{
    Task<UserAccount?> FindByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically inserts an account when its normalized email is absent.
    /// A read-then-insert sequence alone is not sufficient under concurrent registrations.
    /// </summary>
    Task<bool> TryAddAsync(UserAccount account, CancellationToken cancellationToken);

    Task UpdateAsync(UserAccount account, CancellationToken cancellationToken);
}

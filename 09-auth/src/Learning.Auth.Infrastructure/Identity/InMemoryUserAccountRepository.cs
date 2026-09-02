using System.Collections.Concurrent;
using Learning.Auth.Application.Abstractions;
using Learning.Auth.Domain.Users;

namespace Learning.Auth.Infrastructure.Identity;

/// <summary>
/// A concurrency-safe learning adapter. Production persistence must enforce the same normalized
/// email uniqueness with a database constraint and persist account updates transactionally.
/// </summary>
public sealed class InMemoryUserAccountRepository : IUserAccountRepository
{
    private readonly ConcurrentDictionary<string, UserAccount> _accounts =
        new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<Guid, UserAccount> _accountsById = new();

    public Task<UserAccount?> FindByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _accounts.TryGetValue(normalizedEmail, out UserAccount? account);
        return Task.FromResult(account);
    }

    public Task<UserAccount?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _accountsById.TryGetValue(id, out UserAccount? account);
        return Task.FromResult(account);
    }

    public Task<bool> TryAddAsync(UserAccount account, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_accounts.TryAdd(account.Email.NormalizedValue, account))
            return Task.FromResult(false);

        if (!_accountsById.TryAdd(account.Id, account))
        {
            _accounts.TryRemove(account.Email.NormalizedValue, out _);
            throw new InvalidOperationException("A user identifier collision was detected.");
        }
        return Task.FromResult(true);
    }

    public Task UpdateAsync(UserAccount account, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _accounts[account.Email.NormalizedValue] = account;
        return Task.CompletedTask;
    }
}

namespace Learning.Auth.Domain.Users;

public enum AccountStatus
{
    Active,
    Disabled
}

/// <summary>
/// Represents authoritative account state. It deliberately contains no JWT or HTTP concepts.
/// </summary>
public sealed class UserAccount
{
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);

    private UserAccount(Guid id, EmailAddress email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        Status = AccountStatus.Active;
        _roles.Add(RoleNames.Member);
    }

    public Guid Id { get; }

    public EmailAddress Email { get; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public AccountStatus Status { get; private set; }

    public IReadOnlySet<string> Roles => _roles;

    public static UserAccount Register(
        Guid id,
        EmailAddress email,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("A user identifier is required.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        return new UserAccount(id, email, passwordHash, createdAt);
    }

    public void ReplacePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void Disable() => Status = AccountStatus.Disabled;
}

public static class RoleNames
{
    public const string Member = "member";
    public const string Administrator = "administrator";
}

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
    private readonly HashSet<string> _permissions = new(StringComparer.Ordinal);

    private UserAccount(Guid id, EmailAddress email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        Status = AccountStatus.Active;
        _roles.Add(RoleNames.Member);
        _permissions.Add(PermissionNames.ProfileRead);
    }

    public Guid Id { get; }

    public EmailAddress Email { get; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public AccountStatus Status { get; private set; }

    public IReadOnlySet<string> Roles => _roles;

    public IReadOnlySet<string> Permissions => _permissions;

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

    public void GrantRole(string role)
    {
        if (role is not (RoleNames.Member or RoleNames.Administrator))
            throw new ArgumentOutOfRangeException(nameof(role), "Role is not part of the application vocabulary.");
        _roles.Add(role);
    }

    public void GrantPermission(string permission)
    {
        if (permission is not PermissionNames.ProfileRead)
            throw new ArgumentOutOfRangeException(nameof(permission), "Permission is not part of the application vocabulary.");
        _permissions.Add(permission);
    }
}

public static class PermissionNames
{
    public const string ProfileRead = "profile.read";
}

public static class RoleNames
{
    public const string Member = "member";
    public const string Administrator = "administrator";
}

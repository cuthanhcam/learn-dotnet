using Learning.Auth.Domain.Users;

namespace Learning.Auth.UnitTests;

public sealed class UserAuthorizationVocabularyTests
{
    [Fact]
    public void Register_GrantsOnlyBaselineMemberCapabilities()
    {
        UserAccount account = CreateAccount();

        Assert.Equal([RoleNames.Member], account.Roles);
        Assert.Equal([PermissionNames.ProfileRead], account.Permissions);
    }

    [Fact]
    public void GrantRole_UsesClosedApplicationVocabulary()
    {
        UserAccount account = CreateAccount();
        account.GrantRole(RoleNames.Administrator);

        Assert.Contains(RoleNames.Administrator, account.Roles);
        Assert.Throws<ArgumentOutOfRangeException>(() => account.GrantRole("super-user-from-request"));
    }

    private static UserAccount CreateAccount() => UserAccount.Register(Guid.NewGuid(),
        EmailAddress.Create("learner@example.com"), "password-hash-placeholder", DateTimeOffset.UtcNow);
}

using Learning.Auth.Domain.Users;
using Learning.Auth.Infrastructure.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Learning.Auth.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string ProfileRead = "profile-read";
    public const string Administrator = "administrator";

    public static void AddLearningPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(ProfileRead, policy =>
            policy.RequireClaim(JwtClaimNames.Scope, PermissionNames.ProfileRead));
        options.AddPolicy(Administrator, policy =>
            policy.RequireRole(RoleNames.Administrator));
    }
}

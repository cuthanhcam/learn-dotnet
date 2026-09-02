using System.IdentityModel.Tokens.Jwt;
using Learning.Auth.Domain.Documents;
using Learning.Auth.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Learning.Auth.Api.Authorization;

public static class DocumentOperations
{
    public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Publish = new() { Name = nameof(Publish) };
}

/// <summary>
/// Evaluates authorization after the authoritative resource is loaded. Endpoint routes and request
/// bodies are not trusted as ownership evidence.
/// </summary>
public sealed class DocumentAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, LearningDocument>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, LearningDocument resource)
    {
        if (context.User.IsInRole(RoleNames.Administrator))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        bool isOwner = Guid.TryParse(
            context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out Guid userId) &&
            userId == resource.OwnerId;

        if (requirement.Name == DocumentOperations.Read.Name && (resource.IsPublished || isOwner))
            context.Succeed(requirement);
        else if ((requirement.Name == DocumentOperations.Update.Name ||
                  requirement.Name == DocumentOperations.Publish.Name) && isOwner)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

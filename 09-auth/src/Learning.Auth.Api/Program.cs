using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Learning.Auth.Application.Abstractions;
using Learning.Auth.Application.Identity;
using Learning.Auth.Api.Authorization;
using Learning.Auth.Api.Security;
using Learning.Auth.Domain.Documents;
using Learning.Auth.Infrastructure.Documents;
using Learning.Auth.Infrastructure.Identity;
using Learning.Auth.Infrastructure.Sessions;
using Learning.Auth.Infrastructure.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JwtOptions jwtOptions = builder.Configuration.GetRequiredSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? throw new InvalidOperationException("The Jwt configuration section is required.");
jwtOptions.Validate();
SignInSecurityOptions signInSecurityOptions = builder.Configuration
    .GetRequiredSection(SignInSecurityOptions.SectionName)
    .Get<SignInSecurityOptions>() ?? throw new InvalidOperationException(
        "The SignInSecurity configuration section is required.");
signInSecurityOptions.Validate();

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton(signInSecurityOptions);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IUserAccountRepository, InMemoryUserAccountRepository>();
builder.Services.AddSingleton<IPasswordHashService, AspNetCorePasswordHashService>();
builder.Services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
builder.Services.AddSingleton<IRefreshTokenService, CryptographicRefreshTokenService>();
builder.Services.AddSingleton<IRefreshSessionStore, InMemoryRefreshSessionStore>();
builder.Services.AddSingleton<ILearningDocumentRepository, InMemoryLearningDocumentRepository>();
builder.Services.AddTransient<RegistrationService>();
builder.Services.AddTransient<CredentialSignInService>();
builder.Services.AddTransient<SessionSignInService>();
builder.Services.AddTransient<RefreshSessionService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        ValidateLifetime = true,
        RequireExpirationTime = true,
        RequireSignedTokens = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = JwtRegisteredClaimNames.Email,
        RoleClaimType = JwtClaimNames.Role
    };
});
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
builder.Services.AddAuthorization(AuthorizationPolicies.AddLearningPolicies);
builder.Services.AddRateLimiter(AuthRateLimitPolicies.AddAuthRateLimiting);

WebApplication app = builder.Build();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();

app.MapPost("/auth/register", async (RegisterRequest request, RegistrationService registration,
    CancellationToken cancellationToken) =>
{
    try
    {
        RegistrationResult result = await registration.RegisterAsync(
            request.Email, request.Password, cancellationToken);
        return result.Status == RegistrationStatus.Created
            ? Results.Created($"/users/{result.UserId}", new { result.UserId })
            : Results.Conflict(new { message = "The account cannot be created with the supplied identity." });
    }
    catch (ArgumentException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [exception.ParamName ?? "request"] = [exception.Message]
        });
    }
}).AllowAnonymous().RequireRateLimiting(AuthRateLimitPolicies.Credential);

app.MapPost("/auth/sign-in", async (SignInRequest request, SessionSignInService signIn,
    CancellationToken cancellationToken) =>
{
    SessionSignInResult result = await signIn.SignInAsync(
        request.Email, request.Password, cancellationToken);

    // Unknown identities, wrong credentials, and unavailable accounts share one public response.
    return result.Tokens is null
        ? Results.Json(new { message = "Invalid credentials." }, statusCode: StatusCodes.Status401Unauthorized)
        : Results.Ok(TokenResponse.From(result.Tokens));
}).AllowAnonymous().RequireRateLimiting(AuthRateLimitPolicies.Credential);

app.MapPost("/auth/refresh", async (RefreshTokenRequest request, RefreshSessionService refresh,
    CancellationToken cancellationToken) =>
{
    RefreshResult result = await refresh.RefreshAsync(request.RefreshToken, cancellationToken);
    // Replay details are security telemetry, not a distinction exposed to an untrusted caller.
    return result.Tokens is null
        ? Results.Json(new { message = "The refresh credential is invalid." },
            statusCode: StatusCodes.Status401Unauthorized)
        : Results.Ok(TokenResponse.From(result.Tokens));
}).AllowAnonymous().RequireRateLimiting(AuthRateLimitPolicies.Session);

app.MapPost("/auth/revoke", async (RefreshTokenRequest request, RefreshSessionService refresh,
    CancellationToken cancellationToken) =>
{
    await refresh.RevokeAsync(request.RefreshToken, cancellationToken);
    // Idempotent success prevents this endpoint from becoming a token-validity oracle.
    return Results.NoContent();
}).AllowAnonymous().RequireRateLimiting(AuthRateLimitPolicies.Session);

app.MapGet("/auth/me", (ClaimsPrincipal principal) => Results.Ok(new
{
    UserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub),
    Email = principal.FindFirstValue(JwtRegisteredClaimNames.Email),
    Roles = principal.FindAll(JwtClaimNames.Role).Select(claim => claim.Value).ToArray()
})).RequireAuthorization(AuthorizationPolicies.ProfileRead);

app.MapGet("/auth/admin", () => Results.Ok(new { message = "Administrator policy satisfied." }))
    .RequireAuthorization(AuthorizationPolicies.Administrator);

app.MapPost("/documents", async (CreateDocumentRequest request, ClaimsPrincipal principal,
    ILearningDocumentRepository documents, CancellationToken cancellationToken) =>
{
    if (!TryGetUserId(principal, out Guid userId))
        return Results.Unauthorized();

    try
    {
        var document = new LearningDocument(Guid.NewGuid(), userId, request.Title);
        await documents.AddAsync(document, cancellationToken);
        return Results.Created($"/documents/{document.Id}", DocumentResponse.From(document));
    }
    catch (ArgumentException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [exception.ParamName ?? "title"] = [exception.Message]
        });
    }
}).RequireAuthorization();

app.MapGet("/documents/{id:guid}", async (Guid id, ClaimsPrincipal principal,
    ILearningDocumentRepository documents, IAuthorizationService authorization,
    CancellationToken cancellationToken) =>
{
    LearningDocument? document = await documents.FindAsync(id, cancellationToken);
    if (document is null)
        return Results.NotFound();
    AuthorizationResult decision = await authorization.AuthorizeAsync(principal, document, DocumentOperations.Read);
    return decision.Succeeded ? Results.Ok(DocumentResponse.From(document)) : Results.Forbid();
}).RequireAuthorization();

app.MapPut("/documents/{id:guid}", async (Guid id, UpdateDocumentRequest request,
    ClaimsPrincipal principal, ILearningDocumentRepository documents,
    IAuthorizationService authorization, CancellationToken cancellationToken) =>
{
    LearningDocument? document = await documents.FindAsync(id, cancellationToken);
    if (document is null)
        return Results.NotFound();
    AuthorizationResult decision = await authorization.AuthorizeAsync(principal, document, DocumentOperations.Update);
    if (!decision.Succeeded)
        return Results.Forbid();

    try
    {
        document.Rename(request.Title);
        return Results.Ok(DocumentResponse.From(document));
    }
    catch (ArgumentException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [exception.ParamName ?? "title"] = [exception.Message]
        });
    }
}).RequireAuthorization();

app.MapPost("/documents/{id:guid}/publish", async (Guid id, ClaimsPrincipal principal,
    ILearningDocumentRepository documents, IAuthorizationService authorization,
    CancellationToken cancellationToken) =>
{
    LearningDocument? document = await documents.FindAsync(id, cancellationToken);
    if (document is null)
        return Results.NotFound();
    AuthorizationResult decision = await authorization.AuthorizeAsync(principal, document, DocumentOperations.Publish);
    if (!decision.Succeeded)
        return Results.Forbid();

    document.Publish();
    return Results.Ok(DocumentResponse.From(document));
}).RequireAuthorization();

app.Run();

static bool TryGetUserId(ClaimsPrincipal principal, out Guid userId) =>
    Guid.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub), out userId);

public sealed record RegisterRequest(string Email, string Password);
public sealed record SignInRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record CreateDocumentRequest(string Title);
public sealed record UpdateDocumentRequest(string Title);
public sealed record DocumentResponse(Guid Id, Guid OwnerId, string Title, bool IsPublished)
{
    public static DocumentResponse From(LearningDocument document) =>
        new(document.Id, document.OwnerId, document.Title, document.IsPublished);
}
public sealed record TokenResponse(string TokenType, string AccessToken, DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken, DateTimeOffset RefreshTokenExpiresAt)
{
    public static TokenResponse From(SessionTokens tokens) => new("Bearer", tokens.AccessToken.Value,
        tokens.AccessToken.ExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
}

// WebApplicationFactory discovers this type when running integration tests.
public partial class Program;

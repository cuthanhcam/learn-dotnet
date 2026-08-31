using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Learning.Auth.Application.Abstractions;
using Learning.Auth.Application.Identity;
using Learning.Auth.Infrastructure.Identity;
using Learning.Auth.Infrastructure.Sessions;
using Learning.Auth.Infrastructure.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JwtOptions jwtOptions = builder.Configuration.GetRequiredSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? throw new InvalidOperationException("The Jwt configuration section is required.");
jwtOptions.Validate();

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IUserAccountRepository, InMemoryUserAccountRepository>();
builder.Services.AddSingleton<IPasswordHashService, AspNetCorePasswordHashService>();
builder.Services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
builder.Services.AddSingleton<IRefreshTokenService, CryptographicRefreshTokenService>();
builder.Services.AddSingleton<IRefreshSessionStore, InMemoryRefreshSessionStore>();
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
builder.Services.AddAuthorization();

WebApplication app = builder.Build();
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
}).AllowAnonymous();

app.MapPost("/auth/sign-in", async (SignInRequest request, SessionSignInService signIn,
    CancellationToken cancellationToken) =>
{
    SessionSignInResult result = await signIn.SignInAsync(
        request.Email, request.Password, cancellationToken);

    // Unknown identities, wrong credentials, and unavailable accounts share one public response.
    return result.Tokens is null
        ? Results.Json(new { message = "Invalid credentials." }, statusCode: StatusCodes.Status401Unauthorized)
        : Results.Ok(TokenResponse.From(result.Tokens));
}).AllowAnonymous();

app.MapPost("/auth/refresh", async (RefreshTokenRequest request, RefreshSessionService refresh,
    CancellationToken cancellationToken) =>
{
    RefreshResult result = await refresh.RefreshAsync(request.RefreshToken, cancellationToken);
    // Replay details are security telemetry, not a distinction exposed to an untrusted caller.
    return result.Tokens is null
        ? Results.Json(new { message = "The refresh credential is invalid." },
            statusCode: StatusCodes.Status401Unauthorized)
        : Results.Ok(TokenResponse.From(result.Tokens));
}).AllowAnonymous();

app.MapPost("/auth/revoke", async (RefreshTokenRequest request, RefreshSessionService refresh,
    CancellationToken cancellationToken) =>
{
    await refresh.RevokeAsync(request.RefreshToken, cancellationToken);
    // Idempotent success prevents this endpoint from becoming a token-validity oracle.
    return Results.NoContent();
}).AllowAnonymous();

app.MapGet("/auth/me", (ClaimsPrincipal principal) => Results.Ok(new
{
    UserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub),
    Email = principal.FindFirstValue(JwtRegisteredClaimNames.Email),
    Roles = principal.FindAll(JwtClaimNames.Role).Select(claim => claim.Value).ToArray()
})).RequireAuthorization();

app.Run();

public sealed record RegisterRequest(string Email, string Password);
public sealed record SignInRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record TokenResponse(string TokenType, string AccessToken, DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken, DateTimeOffset RefreshTokenExpiresAt)
{
    public static TokenResponse From(SessionTokens tokens) => new("Bearer", tokens.AccessToken.Value,
        tokens.AccessToken.ExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
}

// WebApplicationFactory discovers this type when running integration tests.
public partial class Program;

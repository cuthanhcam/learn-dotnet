using System.Net;
using System.Net.Http.Headers;

namespace Learning.Architecture.IntegrationTests;

public sealed class JwtBoundaryTests
{
    [Fact]
    public async Task ValidSignedAccessToken_CanEnroll()
    {
        using var factory = new EnrollmentApiFactory(realJwt: true);
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", factory.CreateAccessToken());
        using var request = Request();
        using HttpResponseMessage response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("audience")]
    [InlineData("issuer")]
    [InlineData("expiry")]
    [InlineData("signature")]
    public async Task InvalidToken_IsRejectedByTheRealBearerHandler(string fault)
    {
        using var factory = new EnrollmentApiFactory(realJwt: true);
        using HttpClient client = factory.CreateClient();
        string token = factory.CreateAccessToken(
            audience: fault == "audience" ? "another-api" : "architecture-tests",
            issuer: fault == "issuer" ? "https://untrusted.example.test" : "https://identity.example.test",
            expired: fault == "expiry", wrongSignature: fault == "signature");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var request = Request();
        using HttpResponseMessage response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains(response.Headers.WwwAuthenticate, header => header.Scheme == "Bearer");
    }

    [Fact]
    public async Task ValidSignatureWithoutWriteScope_IsForbidden()
    {
        using var factory = new EnrollmentApiFactory(realJwt: true);
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            factory.CreateAccessToken(scope: "catalog.read"));
        using var request = Request();
        using HttpResponseMessage response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static HttpRequestMessage Request()
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            $"/api/v1/offerings/{EnrollmentApiFactory.SampleOffering}/enrollments");
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        return request;
    }
}

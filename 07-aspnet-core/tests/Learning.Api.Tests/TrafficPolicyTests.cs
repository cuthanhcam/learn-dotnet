using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Api.Tests;

public sealed class TrafficPolicyTests
{
    [Fact]
    public async Task Compression_NegotiatesBrotliForCompressibleResponse()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using HttpClient client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/traffic-policy-demo/compressed");
        request.Headers.AcceptEncoding.ParseAdd("br");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("br", response.Content.Headers.ContentEncoding);
        Assert.Contains("Accept-Encoding", response.Headers.Vary);
    }

    [Fact]
    public async Task CorsPreflight_EmitsHeadersOnlyForConfiguredOrigin()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage allowed = await PreflightAsync(client, "https://learn-dotnet.example");
        using HttpResponseMessage denied = await PreflightAsync(client, "https://attacker.example");

        Assert.Equal(HttpStatusCode.NoContent, allowed.StatusCode);
        Assert.Equal("https://learn-dotnet.example", allowed.Headers.GetValues("Access-Control-Allow-Origin").Single());
        Assert.False(denied.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task RateLimiter_RejectsRequestsBeyondPartitionCapacity()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage first = await client.GetAsync("/api/traffic-policy-demo/limited");
        using HttpResponseMessage second = await client.GetAsync("/api/traffic-policy-demo/limited");
        using HttpResponseMessage rejected = await client.GetAsync("/api/traffic-policy-demo/limited");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.Equal("application/problem+json", rejected.Content.Headers.ContentType?.MediaType);
        Assert.True(rejected.Headers.Contains("Retry-After"));
    }

    private static Task<HttpResponseMessage> PreflightAsync(HttpClient client, string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/traffic-policy-demo/compressed");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        return client.SendAsync(request);
    }
}

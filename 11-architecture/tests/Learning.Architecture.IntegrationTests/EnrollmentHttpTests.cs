using System.Net;
using System.Net.Http.Json;
using Learning.Architecture.Api.Features;
using Learning.Architecture.Application.Offerings;

namespace Learning.Architecture.IntegrationTests;

public sealed class EnrollmentHttpTests
{
    [Fact]
    public async Task Catalog_IsPublicAndDoesNotExposeLearnerIdentities()
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = factory.CreateClient();
        OfferingPage page = (await client.GetFromJsonAsync<OfferingPage>("/api/v1/offerings"))!;
        Assert.Equal(2, Assert.Single(page.Items).AvailableSeats);
        string json = await client.GetStringAsync("/api/v1/offerings");
        Assert.DoesNotContain("learner", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AnonymousEnrollment_IsChallenged()
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.PostAsync(Path(), null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("missing", "enrollment.write")]
    [InlineData("not-a-guid", "enrollment.write")]
    [InlineData("00000000-0000-0000-0000-000000000000", "enrollment.write")]
    [InlineData("20000000-0000-0000-0000-000000000001", "catalog.read")]
    public async Task InvalidLearnerOrMissingScope_IsForbidden(string learner, string scope)
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Learner", learner);
        client.DefaultRequestHeaders.Add("X-Test-Scope", scope);
        using HttpResponseMessage response = await client.PostAsync(Path(), null);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SuccessfulRequest_ReplaysSameVersionAndDoesNotConsumeAnotherSeat()
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = AuthorizedClient(factory);
        string key = Guid.NewGuid().ToString();
        using HttpResponseMessage first = await SendAsync(client, key);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        EnrollmentResponse original = (await first.Content.ReadFromJsonAsync<EnrollmentResponse>())!;
        using HttpResponseMessage second = await SendAsync(client, key);
        EnrollmentResponse replay = (await second.Content.ReadFromJsonAsync<EnrollmentResponse>())!;
        Assert.Equal("Enrolled", original.Status);
        Assert.False(original.Replayed);
        Assert.True(replay.Replayed);
        Assert.Equal(original.Version, replay.Version);
        OfferingPage page = (await client.GetFromJsonAsync<OfferingPage>("/api/v1/offerings"))!;
        Assert.Equal(1, Assert.Single(page.Items).EnrolledCount);
    }

    [Fact]
    public async Task AnotherLearnerCannotReuseAnExistingRequestKey()
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient first = AuthorizedClient(factory);
        using HttpClient second = AuthorizedClient(factory);
        string key = Guid.NewGuid().ToString();
        using HttpResponseMessage enrolled = await SendAsync(first, key);
        Assert.Equal(HttpStatusCode.OK, enrolled.StatusCode);
        using HttpResponseMessage reused = await SendAsync(second, key);
        Assert.Equal(HttpStatusCode.Conflict, reused.StatusCode);
    }

    [Fact]
    public async Task ThirdLearnerGetsFullButExistingLearnerCanRepeat()
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient first = AuthorizedClient(factory);
        using HttpClient second = AuthorizedClient(factory);
        using HttpClient third = AuthorizedClient(factory);
        using HttpResponseMessage a = await SendAsync(first, Guid.NewGuid().ToString());
        using HttpResponseMessage b = await SendAsync(second, Guid.NewGuid().ToString());
        Assert.Equal(HttpStatusCode.OK, a.StatusCode);
        Assert.Equal(HttpStatusCode.OK, b.StatusCode);
        using HttpResponseMessage full = await SendAsync(third, Guid.NewGuid().ToString());
        Assert.Equal(HttpStatusCode.Conflict, full.StatusCode);
        using HttpResponseMessage duplicate = await SendAsync(first, Guid.NewGuid().ToString());
        Assert.Equal("AlreadyEnrolled", (await duplicate.Content.ReadFromJsonAsync<EnrollmentResponse>())!.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task MissingOrInvalidKey_IsValidationProblem(string? key)
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = AuthorizedClient(factory);
        using HttpResponseMessage response = await SendAsync(client, key);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task UnknownOffering_IsNotFound()
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = AuthorizedClient(factory);
        using HttpResponseMessage response = await SendAsync(client, Guid.NewGuid().ToString(), Guid.NewGuid());
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("offset=-1")]
    [InlineData("limit=101")]
    [InlineData("limit=not-a-number")]
    public async Task InvalidPagination_IsBadRequest(string query)
    {
        using var factory = new EnrollmentApiFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.GetAsync($"/api/v1/offerings?{query}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static HttpClient AuthorizedClient(EnrollmentApiFactory factory)
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Learner", Guid.NewGuid().ToString());
        client.DefaultRequestHeaders.Add("X-Test-Scope", "catalog.read enrollment.write");
        return client;
    }

    private static string Path(Guid? offering = null) =>
        $"/api/v1/offerings/{offering ?? EnrollmentApiFactory.SampleOffering}/enrollments";

    private static async Task<HttpResponseMessage> SendAsync(HttpClient client, string? key, Guid? offering = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Path(offering));
        if (key is not null)
            request.Headers.Add("Idempotency-Key", key);
        return await client.SendAsync(request);
    }
}

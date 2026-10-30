using System.Net;
using Learning.Architecture.Application.Enrollments;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.IntegrationTests;

public sealed class EnrollmentFailureTests
{
    [Theory]
    [InlineData(99, 1L)]
    [InlineData(4, 1L)]
    [InlineData(0, null)]
    [InlineData(1, 0L)]
    public async Task InvalidWorkflowResult_IsNotReportedAsSuccess(int status, long? version)
    {
        var result = new EnrollmentRequestResult(EnrollmentRequestStatus.Completed,
            new EnrollLearnerResult((EnrollLearnerStatus)status, version));
        using var factory = new EnrollmentApiFactory(requestsOverride: new FixedRequests(result));
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Learner", Guid.NewGuid().ToString());
        client.DefaultRequestHeaders.Add("X-Test-Scope", "enrollment.write");
        client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString());
        using HttpResponseMessage response = await client.PostAsync(
            $"/api/v1/offerings/{EnrollmentApiFactory.SampleOffering}/enrollments", null);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Null(response.Headers.RetryAfter);
    }

    [Theory]
    [InlineData(5, false, 503)]
    [InlineData(6, false, 503)]
    [InlineData(5, true, 503)]
    [InlineData(6, true, 503)]
    [InlineData(19, false, 500)]
    [InlineData(19, true, 500)]
    public async Task ProviderFailure_IsClassifiedWithoutLeakingDetails(int code, bool wrapped, int expectedStatus)
    {
        Exception failure = new SqliteException("private database diagnostic", code);
        if (wrapped)
            failure = new DbUpdateException("private EF diagnostic", failure);
        using var factory = new EnrollmentApiFactory(requestsOverride: new FailingRequests(failure));
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Learner", Guid.NewGuid().ToString());
        client.DefaultRequestHeaders.Add("X-Test-Scope", "enrollment.write");
        client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString());

        using HttpResponseMessage response = await client.PostAsync(
            $"/api/v1/offerings/{EnrollmentApiFactory.SampleOffering}/enrollments", null);

        Assert.Equal((HttpStatusCode)expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        if (expectedStatus == 503)
            Assert.Equal(TimeSpan.FromSeconds(1), response.Headers.RetryAfter!.Delta);
        else
            Assert.Null(response.Headers.RetryAfter);
        string body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("private", body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FailingRequests(Exception failure) : IEnrollmentRequests
    {
        public ValueTask<EnrollmentRequestResult> ExecuteAsync(EnrollmentRequest request,
            CancellationToken cancellationToken = default) => ValueTask.FromException<EnrollmentRequestResult>(failure);
    }

    private sealed class FixedRequests(EnrollmentRequestResult result) : IEnrollmentRequests
    {
        public ValueTask<EnrollmentRequestResult> ExecuteAsync(EnrollmentRequest request,
            CancellationToken cancellationToken = default) => ValueTask.FromResult(result);
    }
}

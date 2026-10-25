using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Learning.Testing.Api;

namespace Learning.Testing.IntegrationTests;

[Trait("Category", "Integration")]
public sealed class ReminderHttpTests
{
    [Fact]
    public async Task PostReminder_DueLoan_ReturnsSerializedOutcomeAndUsesConfiguredClock()
    {
        using var factory = new ReminderApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(PathFor(OfflineLoanReader.DueLoanId), null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(OfflineLoanReader.DueLoanId, body.RootElement.GetProperty("loanId").GetGuid());
        Assert.Equal("Sent", body.RootElement.GetProperty("outcome").GetString());
        var attempt = Assert.Single(factory.Sender.Attempts);
        Assert.Equal(OfflineLoanReader.DueLoanId, attempt.LoanId);
        Assert.Equal(ReminderApiFactory.Now, attempt.EvaluatedAt);
        Assert.Equal(DateTimeOffset.UnixEpoch, attempt.DueAt);
    }

    [Theory]
    [InlineData("20000000-0000-0000-0000-000000000002")]
    [InlineData("20000000-0000-0000-0000-000000000003")]
    public async Task PostReminder_UnneededLoan_ReturnsNotRequiredWithoutSending(string loanId)
    {
        using var factory = new ReminderApiFactory();
        using var client = factory.CreateClient();
        Guid id = Guid.Parse(loanId);

        using var response = await client.PostAsync(PathFor(id), null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new ReminderResponse(id, "NotRequired"), await response.Content.ReadFromJsonAsync<ReminderResponse>());
        Assert.Empty(factory.Sender.Attempts);
    }

    [Theory]
    [InlineData("20000000-0000-0000-0000-000000000004", 404, "Loan not found.")]
    [InlineData("00000000-0000-0000-0000-000000000000", 400, "A nonempty loan ID is required.")]
    public async Task PostReminder_InvalidOrMissingId_ReturnsProblemDetails(string loanId, int status, string title)
    {
        using var factory = new ReminderApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(PathFor(Guid.Parse(loanId)), null);

        Assert.Equal(status, (int)response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(status, body.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(title, body.RootElement.GetProperty("title").GetString());
        Assert.Empty(factory.Sender.Attempts);
    }

    [Fact]
    public async Task PostReminder_MalformedRouteId_DoesNotMatchGuidRoute()
    {
        using var factory = new ReminderApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsync("/loans/not-a-guid/reminders", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(factory.Sender.Attempts);
    }

    [Fact]
    public async Task GetReminder_PostOnlyRoute_RejectsWrongVerb()
    {
        using var factory = new ReminderApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(PathFor(OfflineLoanReader.DueLoanId));

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        Assert.Empty(factory.Sender.Attempts);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PostReminder_DependencyFailure_ReturnsGenericProblemWithoutInternalDetails(bool readerFails)
    {
        const string diagnostic = "Internal adapter diagnostic for the test.";
        var failure = new IOException(diagnostic);
        using var factory = readerFails
            ? new ReminderApiFactory { ReaderOverride = new FaultingLoanReader(failure) }
            : new ReminderApiFactory { SenderOverride = _ => Task.FromException(failure) };
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(PathFor(OfflineLoanReader.DueLoanId), null);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        string payload = await response.Content.ReadAsStringAsync();
        using var body = JsonDocument.Parse(payload);
        Assert.Equal(500, body.RootElement.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(body.RootElement.GetProperty("title").GetString()));
        Assert.DoesNotContain(diagnostic, payload);
        Assert.Equal(readerFails ? 0 : 1, factory.Sender.Attempts.Count);
    }

    [Fact]
    public async Task PostReminder_SeparateHosts_DoNotShareAttemptHistory()
    {
        using var first = new ReminderApiFactory();
        using var second = new ReminderApiFactory();
        using var client = first.CreateClient();
        using var otherClient = second.CreateClient();

        using var response = await client.PostAsync(PathFor(OfflineLoanReader.DueLoanId), null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(first.Sender.Attempts);
        Assert.Empty(second.Sender.Attempts);
    }

    [Fact]
    public async Task PostReminder_RepeatedRequest_AttemptsTwiceBecauseThereIsNoDeduplication()
    {
        using var factory = new ReminderApiFactory();
        using var client = factory.CreateClient();

        using var first = await client.PostAsync(PathFor(OfflineLoanReader.DueLoanId), null);
        using var second = await client.PostAsync(PathFor(OfflineLoanReader.DueLoanId), null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(2, factory.Sender.Attempts.Count);
    }

    [Fact]
    public async Task PostReminder_ClientAbortsPendingRead_PropagatesRequestCancellationWithoutSending()
    {
        var reader = new BlockingLoanReader();
        using var factory = new ReminderApiFactory { ReaderOverride = reader };
        using var client = factory.CreateClient();
        using var cancellation = new CancellationTokenSource();

        Task<HttpResponseMessage> pending = client.PostAsync(PathFor(OfflineLoanReader.DueLoanId), null, cancellation.Token);
        await reader.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);
        await reader.CancellationObserved.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Empty(factory.Sender.Attempts);
    }

    private static string PathFor(Guid loanId) => $"/loans/{loanId}/reminders";
}

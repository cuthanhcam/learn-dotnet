using System.Net;
using System.Net.Http.Json;
using Learning.Api.BackgroundJobs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Learning.Api.Tests;

public sealed class BackgroundJobTests
{
    [Fact]
    public async Task SubmittedJob_TransitionsToCompletedState()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage accepted = await client.PostAsJsonAsync(
            "/api/background-jobs",
            new SubmitBackgroundJobRequest { Description = "Generate learning summary" });
        BackgroundJobState submitted =
            (await accepted.Content.ReadFromJsonAsync<BackgroundJobState>())!;

        Assert.Equal(HttpStatusCode.Accepted, accepted.StatusCode);
        BackgroundJobState completed = await WaitForStatusAsync(
            client, submitted.Id, BackgroundJobStatus.Completed);
        Assert.NotNull(completed.StartedAt);
        Assert.NotNull(completed.CompletedAt);
    }

    [Fact]
    public async Task FullQueue_RejectsInsteadOfGrowingWithoutBound()
    {
        var processor = new BlockingProcessor();
        await using WebApplicationFactory<Program> factory = CreateFactory(processor);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage running = await SubmitAsync(client, "Running job");
        await processor.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        using HttpResponseMessage queuedOne = await SubmitAsync(client, "Queued job one");
        using HttpResponseMessage queuedTwo = await SubmitAsync(client, "Queued job two");
        using HttpResponseMessage rejected = await SubmitAsync(client, "Rejected job");

        Assert.Equal(HttpStatusCode.Accepted, running.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, queuedOne.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, queuedTwo.StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, rejected.StatusCode);

        processor.Release.TrySetResult();
    }

    [Fact]
    public async Task Shutdown_WaitsForAcceptedWorkToDrainWithinDeadline()
    {
        var processor = new BlockingProcessor();
        WebApplicationFactory<Program> factory = CreateFactory(processor);
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage accepted = await SubmitAsync(client, "Drain during shutdown");
        accepted.EnsureSuccessStatusCode();
        await processor.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Task disposing = factory.DisposeAsync().AsTask();
        await Task.Delay(50);
        Assert.False(disposing.IsCompleted);

        processor.Release.TrySetResult();
        await disposing.WaitAsync(TimeSpan.FromSeconds(5));
    }

    private static WebApplicationFactory<Program> CreateFactory(IBackgroundJobProcessor processor) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IBackgroundJobProcessor>();
                services.AddScoped(_ => processor);
            }));

    private static Task<HttpResponseMessage> SubmitAsync(HttpClient client, string description) =>
        client.PostAsJsonAsync(
            "/api/background-jobs",
            new SubmitBackgroundJobRequest { Description = description });

    private static async Task<BackgroundJobState> WaitForStatusAsync(
        HttpClient client,
        Guid id,
        BackgroundJobStatus expected)
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            BackgroundJobState? state = await client.GetFromJsonAsync<BackgroundJobState>(
                $"/api/background-jobs/{id}");
            if (state?.Status == expected)
            {
                return state;
            }

            await Task.Delay(20);
        }

        throw new TimeoutException($"Job {id} did not reach {expected}.");
    }

    private sealed class BlockingProcessor : IBackgroundJobProcessor
    {
        public TaskCompletionSource Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Release { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task ProcessAsync(BackgroundJob job, CancellationToken cancellationToken)
        {
            Started.TrySetResult();
            await Release.Task.WaitAsync(cancellationToken);
        }
    }
}

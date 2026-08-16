using System.Diagnostics.Metrics;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using Learning.Api.BackgroundJobs;
using Learning.Api.Operations;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Api.Tests;

public sealed class MetricsTests
{
    [Fact]
    public async Task AcceptedJob_EmitsBoundedOutcomeMetric()
    {
        var measurements = new ConcurrentQueue<(long Value, string? Outcome)>();
        using var listener = new MeterListener
        {
            InstrumentPublished = (instrument, meterListener) =>
            {
                if (instrument.Meter.Name == LearningMetrics.MeterName)
                {
                    meterListener.EnableMeasurementEvents(instrument);
                }
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
        {
            string? outcome = null;
            foreach (KeyValuePair<string, object?> tag in tags)
            {
                if (tag.Key == "outcome")
                {
                    outcome = tag.Value?.ToString();
                }
            }

            if (instrument.Name == "learning.background_jobs.submissions")
            {
                measurements.Enqueue((value, outcome));
            }
        });
        listener.Start();

        await using var factory = new WebApplicationFactory<Program>();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/background-jobs",
            new SubmitBackgroundJobRequest { Description = "Measure accepted work" });
        response.EnsureSuccessStatusCode();

        Assert.Contains(measurements, measurement =>
            measurement is { Value: 1, Outcome: "accepted" });
    }
}

using System.Diagnostics.Metrics;

namespace Learning.Api.Operations;

public sealed class LearningMetrics : IDisposable
{
    public const string MeterName = "Learning.Api";
    private readonly Meter _meter = new(MeterName, "1.0.0");
    private readonly Counter<long> _productsCreated;
    private readonly Counter<long> _backgroundJobs;

    public LearningMetrics()
    {
        _productsCreated = _meter.CreateCounter<long>(
            "learning.products.created",
            unit: "{product}",
            description: "Products successfully created through the API.");
        _backgroundJobs = _meter.CreateCounter<long>(
            "learning.background_jobs.submissions",
            unit: "{job}",
            description: "Background job submissions grouped by bounded outcome.");
    }

    public void ProductCreated() => _productsCreated.Add(1);

    public void BackgroundJobSubmitted(string outcome) =>
        // 'accepted' and 'rejected' are a closed low-cardinality set. Never attach job IDs,
        // descriptions, user IDs, or correlation IDs as metric tags.
        _backgroundJobs.Add(1, new KeyValuePair<string, object?>("outcome", outcome));

    public void Dispose() => _meter.Dispose();
}

using System.Globalization;
using Learning.Patterns.Behavioral.Observer.Baseline;
using Learning.Patterns.Behavioral.Observer.Observers;
using Learning.Patterns.Behavioral.Observer.Refactored;

namespace Learning.Patterns.Behavioral.Observer;

public static class ObserverDemo
{
    public static void Run(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        var first = new PriceUpdate("BOOK-1", 12m);
        var second = new PriceUpdate("BOOK-1", 20m);
        var baselineDisplay = new PriceDisplay();
        var baseline = new FixedPricePublisher(baselineDisplay, new PriceThresholdObserver(15m));
        baseline.Publish(first);
        baseline.Publish(second);

        var feed = new PriceFeed();
        var display = new PriceDisplay();
        var threshold = new PriceThresholdObserver(15m);
        using IDisposable screen = feed.Subscribe(display);
        using IDisposable alert = feed.Subscribe(threshold);
        feed.Publish(first);
        screen.Dispose(); // The screen's lifetime ends; the other subscription remains active.
        feed.Publish(second);
        feed.Complete();

        output.WriteLine($"Baseline latest USD: {baselineDisplay.Latest!.PriceUsd.ToString("0.00", CultureInfo.InvariantCulture)}");
        output.WriteLine($"Unsubscribed display USD: {display.Latest!.PriceUsd.ToString("0.00", CultureInfo.InvariantCulture)}");
        output.WriteLine($"Threshold matches: {threshold.Matches}");
        output.WriteLine($"Active observer completed: {threshold.IsStopped}");
        var late = new PriceDisplay();
        using IDisposable ignored = feed.Subscribe(late);
        output.WriteLine($"Late observer completed: {late.IsStopped}");
    }
}

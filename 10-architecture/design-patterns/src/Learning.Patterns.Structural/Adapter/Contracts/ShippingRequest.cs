namespace Learning.Patterns.Structural.Adapter.Contracts;

public sealed record ShippingRequest
{
    public ShippingRequest(decimal weightKilograms, string destinationCountry)
    {
        // Exact gram conversion is possible only for supported precision. Do not silently round a
        // heavier parcel down to obtain a cheaper provider quote.
        if (weightKilograms is <= 0 or > 1000 || decimal.Round(weightKilograms, 3) != weightKilograms)
            throw new ArgumentOutOfRangeException(nameof(weightKilograms), "Weight must be 0.001..1000 kg in whole grams.");
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationCountry);
        string country = destinationCountry.Trim().ToUpperInvariant();
        if (country.Length != 2 || !country.All(char.IsAsciiLetterUpper))
            throw new ArgumentException("Use a two-letter ASCII country code.", nameof(destinationCountry));
        WeightKilograms = weightKilograms;
        DestinationCountry = country;
    }

    public decimal WeightKilograms { get; }
    public string DestinationCountry { get; }
}

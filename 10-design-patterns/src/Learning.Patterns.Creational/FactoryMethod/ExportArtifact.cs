using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod;

public sealed record ExportArtifact(string MediaType, string Content)
{
    public static ExportArtifact Render(ReportDocument document, IReportFormatter formatter)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(formatter);
        string content = formatter.Format(document);
        // A formatter is an extension point; capture its metadata once so the returned artifact
        // contains the exact value we validated even if the product has a stateful property.
        string mediaType = formatter.MediaType;
        if (string.IsNullOrWhiteSpace(mediaType) || string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("A formatter must produce a media type and nonblank content.");
        return new ExportArtifact(mediaType, content);
    }
}

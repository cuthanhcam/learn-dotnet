using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod;

public sealed record ExportArtifact(string MediaType, string Content)
{
    public static ExportArtifact Render(ReportDocument document, IReportFormatter formatter)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(formatter);
        string content = formatter.Format(document);
        if (string.IsNullOrWhiteSpace(formatter.MediaType) || string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("A formatter must produce a media type and nonblank content.");
        return new ExportArtifact(formatter.MediaType, content);
    }
}

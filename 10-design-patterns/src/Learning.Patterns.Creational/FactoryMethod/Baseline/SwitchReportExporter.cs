using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod.Baseline;

public enum ReportFormat { Text, Json }

/// <summary>
/// A correct simple selection factory embedded in a small workflow. New formats require editing
/// this switch. That is reasonable for a closed list, but not for a creator extension point.
/// </summary>
public sealed class SwitchReportExporter
{
    public ExportArtifact Export(ReportDocument document, ReportFormat format)
    {
        ArgumentNullException.ThrowIfNull(document);
        IReportFormatter formatter = format switch
        {
            ReportFormat.Text => new TextReportFormatter(),
            ReportFormat.Json => new JsonReportFormatter(),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
        return ExportArtifact.Render(document, formatter);
    }
}

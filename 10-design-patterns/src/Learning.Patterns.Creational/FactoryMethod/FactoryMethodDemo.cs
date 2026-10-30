using System.Text.Json;
using Learning.Patterns.Creational.FactoryMethod.Alternatives;
using Learning.Patterns.Creational.FactoryMethod.Products;
using Learning.Patterns.Creational.FactoryMethod.Refactored;

namespace Learning.Patterns.Creational.FactoryMethod;

public static class FactoryMethodDemo
{
    public static void Run(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        var document = new ReportDocument("Architecture report", ["Keep creation policy explicit."]);
        ExportArtifact text = new TextReportExporter().Export(document);
        ExportArtifact json = new JsonReportExporter().Export(document);
        using JsonDocument parsed = JsonDocument.Parse(json.Content);
        output.WriteLine($"Text media type: {text.MediaType}");
        output.WriteLine($"Text first line: {text.Content.Split('\n')[0]}");
        output.WriteLine($"JSON title: {parsed.RootElement.GetProperty("title").GetString()}");
        output.WriteLine($"Injected formatter: {new InjectedReportExporter(new TextReportFormatter()).Export(document).MediaType}");
    }
}

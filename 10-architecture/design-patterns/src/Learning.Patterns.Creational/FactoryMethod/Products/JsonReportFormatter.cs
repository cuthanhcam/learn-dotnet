using System.Text.Json;

namespace Learning.Patterns.Creational.FactoryMethod.Products;

public sealed class JsonReportFormatter : IReportFormatter
{
    public string MediaType => "application/json";

    public string Format(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        // Use the serializer, never concatenate user-supplied title/paragraph text into JSON syntax.
        return JsonSerializer.Serialize(new { title = document.Title, paragraphs = document.Paragraphs });
    }
}

namespace Learning.Patterns.Creational.FactoryMethod.Products;

public sealed class TextReportFormatter : IReportFormatter
{
    public string MediaType => "text/plain";

    public string Format(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        // Stable LF line endings are part of this format contract, independent of the host OS.
        return document.Title + "\n\n" + string.Join("\n", document.Paragraphs);
    }
}

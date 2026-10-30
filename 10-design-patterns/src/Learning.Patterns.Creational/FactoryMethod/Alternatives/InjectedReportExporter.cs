using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod.Alternatives;

/// <summary>
/// Composition alternative: the caller supplies a product rather than subclassing a creator. Often
/// simpler in DI-based applications. This does not claim to be the canonical GoF Factory Method.
/// The formatter is borrowed; no disposable-resource ownership is introduced by this example.
/// </summary>
public sealed class InjectedReportExporter
{
    private readonly IReportFormatter _formatter;

    public InjectedReportExporter(IReportFormatter formatter) =>
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));

    public ExportArtifact Export(ReportDocument document) => ExportArtifact.Render(document, _formatter);
}

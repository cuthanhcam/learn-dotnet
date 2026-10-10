using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod.Refactored;

/// <summary>
/// Creator: the public workflow validates input and renders an artifact. Its protected Factory Method
/// lets subclasses choose the product the workflow uses. A static method named Create is not the same
/// collaboration. Export is nonvirtual so extension stays at the intended creation hook.
/// </summary>
public abstract class ReportExporter
{
    public ExportArtifact Export(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        IReportFormatter formatter = CreateFormatter()
            ?? throw new InvalidOperationException("The factory method returned no formatter.");
        return ExportArtifact.Render(document, formatter);
    }

    protected abstract IReportFormatter CreateFormatter();
}

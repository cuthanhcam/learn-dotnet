namespace Learning.Patterns.Creational.FactoryMethod.Products;

/// <summary>
/// Product contract. Format produces in-memory content, not a filesystem/network side effect. The
/// creator owns the workflow; products own format-specific encoding. Products here own no resources.
/// </summary>
public interface IReportFormatter
{
    string MediaType { get; }
    string Format(ReportDocument document);
}

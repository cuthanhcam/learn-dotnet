using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod.Refactored;

public sealed class JsonReportExporter : ReportExporter
{
    protected override IReportFormatter CreateFormatter() => new JsonReportFormatter();
}

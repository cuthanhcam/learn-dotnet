using Learning.Patterns.Creational.FactoryMethod.Products;

namespace Learning.Patterns.Creational.FactoryMethod.Refactored;

public sealed class TextReportExporter : ReportExporter
{
    protected override IReportFormatter CreateFormatter() => new TextReportFormatter();
}

using System.Text.Json;
using Learning.Patterns.Creational.FactoryMethod;
using Learning.Patterns.Creational.FactoryMethod.Alternatives;
using Learning.Patterns.Creational.FactoryMethod.Baseline;
using Learning.Patterns.Creational.FactoryMethod.Products;
using Learning.Patterns.Creational.FactoryMethod.Refactored;

namespace Learning.Patterns.Creational.Tests.FactoryMethod;

public sealed class ReportExporterTests
{
    [Theory]
    [InlineData(ReportFormat.Text)]
    [InlineData(ReportFormat.Json)]
    public void Refactor_PreservesBaselineAndInjectedAlternative(ReportFormat format)
    {
        var document = new ReportDocument("Report", ["First", "Second"]);
        ReportExporter creator = format == ReportFormat.Text ? new TextReportExporter() : new JsonReportExporter();
        IReportFormatter product = format == ReportFormat.Text ? new TextReportFormatter() : new JsonReportFormatter();
        Assert.Equal(new SwitchReportExporter().Export(document, format), creator.Export(document));
        Assert.Equal(new InjectedReportExporter(product).Export(document), creator.Export(document));
    }

    [Fact]
    public void Text_HasExplicitLineEndingAndParagraphOrder()
    {
        ExportArtifact artifact = new TextReportExporter().Export(new ReportDocument(" Report ", ["First", "Second"]));
        Assert.Equal("text/plain", artifact.MediaType);
        Assert.Equal("Report\n\nFirst\nSecond", artifact.Content);
    }

    [Fact]
    public void Json_RoundTripsQuotesNewlinesAndUnicode()
    {
        var document = new ReportDocument("A \"quoted\" report", ["Line one\nLine two", "Unicode: λ"]);
        ExportArtifact artifact = new JsonReportExporter().Export(document);
        using JsonDocument json = JsonDocument.Parse(artifact.Content);
        Assert.Equal("application/json", artifact.MediaType);
        Assert.Equal(document.Title, json.RootElement.GetProperty("title").GetString());
        Assert.Equal(document.Paragraphs, json.RootElement.GetProperty("paragraphs").EnumerateArray().Select(item => item.GetString()!));
    }

    [Fact]
    public void NewCreator_ExtendsWorkflowWithoutEditingBaseCreatorOrSwitch()
    {
        var creator = new CustomExporter();
        Assert.Equal("REPORT", creator.Export(new ReportDocument("Report", ["Body"])).Content);
        Assert.Equal(1, creator.ProductsCreated);
    }

    [Fact]
    public void FactoryHook_IsInvokedPerExportNotCachedByTheBaseWorkflow()
    {
        var creator = new CustomExporter();
        var document = new ReportDocument("Report", ["Body"]);
        creator.Export(document);
        creator.Export(document);
        Assert.Equal(2, creator.ProductsCreated);
    }

    [Fact]
    public void InvalidInput_IsRejectedBeforeConstructingAProduct()
    {
        var creator = new CustomExporter();
        Assert.Throws<ArgumentNullException>(() => creator.Export(null!));
        Assert.Equal(0, creator.ProductsCreated);
    }

    [Fact]
    public void MissingProduct_IsAContractViolation() => Assert.Throws<InvalidOperationException>(() =>
        new MissingProductExporter().Export(new ReportDocument("Report", ["Body"])));

    [Fact]
    public void EmptyProductOutput_IsAContractViolation() => Assert.Throws<InvalidOperationException>(() =>
        new InjectedReportExporter(new EmptyFormatter()).Export(new ReportDocument("Report", ["Body"])));

    [Fact]
    public void UnknownBaselineFormat_IsNotSilentlyMappedToText() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new SwitchReportExporter().Export(new ReportDocument("Report", ["Body"]), (ReportFormat)999));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void BlankTitle_IsRejected(string title) => Assert.Throws<ArgumentException>(() => new ReportDocument(title, ["Body"]));

    [Fact]
    public void OversizedTitle_IsRejected() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new ReportDocument(new string('a', 101), ["Body"]));

    [Fact]
    public void EmptyParagraphCollection_IsRejected() => Assert.Throws<ArgumentException>(() => new ReportDocument("Report", []));

    [Fact]
    public void OversizedParagraphCollection_IsRejected() => Assert.Throws<ArgumentException>(() =>
        new ReportDocument("Report", Enumerable.Repeat("Body", 101)));

    [Fact]
    public void BlankParagraph_IsRejected() => Assert.Throws<ArgumentException>(() => new ReportDocument("Report", [" "]));

    [Fact]
    public void OversizedParagraph_IsRejected() => Assert.Throws<ArgumentException>(() =>
        new ReportDocument("Report", [new string('a', 1001)]));

    [Fact]
    public void InputCollection_IsCopiedBeforeLaterCallerMutation()
    {
        string[] paragraphs = ["Original"];
        var document = new ReportDocument("Report", paragraphs);
        paragraphs[0] = "Changed";
        Assert.Equal("Original", Assert.Single(document.Paragraphs));
    }

    [Fact]
    public void Demo_IsDeterministic()
    {
        using var output = new StringWriter();
        FactoryMethodDemo.Run(output);
        Assert.Equal(new[]
        {
            "Text media type: text/plain", "Text first line: Architecture report",
            "JSON title: Architecture report", "Injected formatter: text/plain"
        }, output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }

    private sealed class CustomExporter : ReportExporter
    {
        public int ProductsCreated { get; private set; }
        protected override IReportFormatter CreateFormatter()
        {
            ProductsCreated++;
            return new UppercaseFormatter();
        }
    }

    private sealed class UppercaseFormatter : IReportFormatter
    {
        public string MediaType => "text/plain";
        public string Format(ReportDocument document) => document.Title.ToUpperInvariant();
    }

    private sealed class MissingProductExporter : ReportExporter
    {
        protected override IReportFormatter CreateFormatter() => null!;
    }

    private sealed class EmptyFormatter : IReportFormatter
    {
        public string MediaType => "text/plain";
        public string Format(ReportDocument document) => "";
    }
}

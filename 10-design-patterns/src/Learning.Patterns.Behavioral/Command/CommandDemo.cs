using Learning.Patterns.Behavioral.Command.Baseline;
using Learning.Patterns.Behavioral.Command.ModernCSharp;

namespace Learning.Patterns.Behavioral.Command;

public static class CommandDemo
{
    public static void Run(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        var baseline = new TextDocument();
        new DirectEditor(baseline).Append("Learn .NET");

        var document = new TextDocument();
        var history = new EditHistory();
        IEditCommand prefix = new AppendTextCommand(document, "Learn");
        IEditCommand suffix = new AppendTextCommand(document, " .NET");
        history.Execute(prefix);
        history.Execute(suffix);
        output.WriteLine($"Baseline: {baseline.Text}");
        output.WriteLine($"Command: {document.Text}");
        history.Undo();
        output.WriteLine($"After undo: {document.Text}");
        history.Execute(suffix);
        output.WriteLine($"Re-executed: {document.Text}");

        var simple = new TextDocument();
        new DelegateButton(() => simple.Append("Callback")).Click();
        output.WriteLine($"Delegate: {simple.Text}");
    }
}

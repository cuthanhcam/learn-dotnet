using Learning.Patterns.Behavioral.Command;
using Learning.Patterns.Behavioral.Command.Baseline;
using Learning.Patterns.Behavioral.Command.ModernCSharp;
using Xunit;

namespace Learning.Patterns.Behavioral.Tests.Command;

public sealed class EditHistoryTests
{
    [Fact]
    public void Construction_DefersExecution()
    {
        var document = new TextDocument();
        var command = new AppendTextCommand(document, "A");
        Assert.Equal("", document.Text);
        command.Execute();
        Assert.Equal("A", document.Text);
    }

    [Fact]
    public void BaselineAndCommand_PreserveImmediateAppendBehavior()
    {
        var baseline = new TextDocument();
        var refactored = new TextDocument();
        new DirectEditor(baseline).Append("Learn .NET");
        new EditHistory().Execute(new AppendTextCommand(refactored, "Learn .NET"));
        Assert.Equal(baseline.Text, refactored.Text);
    }

    [Fact]
    public void Undo_RestoresMultipleEditsInReverseOrder()
    {
        var document = new TextDocument();
        var history = new EditHistory();
        history.Execute(new AppendTextCommand(document, "A"));
        history.Execute(new AppendTextCommand(document, "B"));
        Assert.Equal("AB", document.Text);
        Assert.True(history.Undo());
        Assert.Equal("A", document.Text);
        Assert.True(history.Undo());
        Assert.Equal("", document.Text);
        Assert.False(history.Undo());
        Assert.Equal(0, history.Count);
    }

    [Fact]
    public void RepeatedExecution_IsNotIdempotentAndHasIndependentReceipts()
    {
        var document = new TextDocument();
        var command = new AppendTextCommand(document, "A");
        var history = new EditHistory();
        history.Execute(command);
        history.Execute(command);
        Assert.Equal("AA", document.Text);
        history.Undo();
        history.Undo();
        Assert.Equal("", document.Text);
        history.Execute(command);
        Assert.Equal("A", document.Text);
    }

    [Fact]
    public void FullHistory_RejectsBeforeExecutingAndUndoFreesCapacity()
    {
        var document = new TextDocument();
        var history = new EditHistory(1);
        history.Execute(new AppendTextCommand(document, "A"));
        Assert.Throws<InvalidOperationException>(() => history.Execute(new AppendTextCommand(document, "B")));
        Assert.Equal("A", document.Text);
        Assert.Equal(1, history.Count);
        history.Undo();
        history.Execute(new AppendTextCommand(document, "B"));
        Assert.Equal("B", document.Text);
    }

    [Fact]
    public void ReceiverFailure_PreservesDocumentAndHistory()
    {
        var document = new TextDocument();
        document.Append(new string('x', TextDocument.MaximumLength));
        var history = new EditHistory();
        Assert.Throws<InvalidOperationException>(() => history.Execute(new AppendTextCommand(document, "A")));
        Assert.Equal(TextDocument.MaximumLength, document.Text.Length);
        Assert.Equal(0, history.Count);
        Assert.False(history.Undo());
    }

    [Fact]
    public void ExternalEdit_ConflictsWithoutPoppingHistory()
    {
        var document = new TextDocument();
        var history = new EditHistory();
        history.Execute(new AppendTextCommand(document, "A"));
        document.Append("B");
        Assert.Throws<InvalidOperationException>(() => history.Undo());
        Assert.Equal("AB", document.Text);
        Assert.Equal(1, history.Count);
        Assert.Throws<InvalidOperationException>(() => history.Undo());
    }

    [Fact]
    public void Receipt_CanOnlyUndoOnce()
    {
        var document = new TextDocument();
        IUndoEdit receipt = new AppendTextCommand(document, "A").Execute();
        receipt.Undo();
        Assert.Throws<InvalidOperationException>(() => receipt.Undo());
        Assert.Equal("", document.Text);
    }

    [Fact]
    public void OutOfOrderUndo_CanRecoverAfterLaterEditIsUndone()
    {
        var document = new TextDocument();
        IUndoEdit first = new AppendTextCommand(document, "A").Execute();
        IUndoEdit second = new AppendTextCommand(document, "B").Execute();
        Assert.Throws<InvalidOperationException>(() => first.Undo());
        second.Undo();
        first.Undo();
        Assert.Equal("", document.Text);
    }

    [Fact]
    public void EqualTextFromAnotherEdit_DoesNotAuthorizeOldReceipt()
    {
        var document = new TextDocument();
        IUndoEdit original = new AppendTextCommand(document, "A").Execute();
        original.Undo();
        document.Append("A");
        Assert.Throws<InvalidOperationException>(() => original.Undo());
        Assert.Equal("A", document.Text);
    }

    [Fact]
    public void DistinctDocuments_HaveIndependentHistories()
    {
        var first = new TextDocument();
        var second = new TextDocument();
        var history = new EditHistory();
        history.Execute(new AppendTextCommand(first, "A"));
        history.Execute(new AppendTextCommand(second, "B"));
        history.Undo();
        Assert.Equal("A", first.Text);
        Assert.Equal("", second.Text);
    }

    [Fact]
    public void NestedExecution_IsRejectedAndInvokerRecovers()
    {
        var history = new EditHistory();
        var document = new TextDocument();
        var nested = new CallbackCommand(() => history.Execute(new AppendTextCommand(document, "nested")));
        Assert.Throws<InvalidOperationException>(() => history.Execute(nested));
        Assert.Equal("", document.Text);
        Assert.Equal(0, history.Count);
        history.Execute(new AppendTextCommand(document, "valid"));
        Assert.Equal("valid", document.Text);
    }

    [Fact]
    public void NestedUndo_IsRejectedBeforeHistoryChanges()
    {
        var history = new EditHistory();
        var document = new TextDocument();
        history.Execute(new AppendTextCommand(document, "A"));
        Assert.Throws<InvalidOperationException>(() => history.Execute(new CallbackCommand(() => history.Undo())));
        Assert.Equal(1, history.Count);
        Assert.Equal("A", document.Text);
        Assert.True(history.Undo());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(129)]
    public void InvalidCapacity_IsRejected(int capacity) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new EditHistory(capacity));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void InvalidText_IsRejectedWithoutMutation(string? text)
    {
        var document = new TextDocument();
        Assert.ThrowsAny<ArgumentException>(() => new AppendTextCommand(document, text!));
        Assert.ThrowsAny<ArgumentException>(() => document.Append(text!));
        Assert.Equal("", document.Text);
    }

    [Fact]
    public void ExactDocumentLimit_IsAcceptedAndReversible()
    {
        var document = new TextDocument();
        IUndoEdit receipt = new AppendTextCommand(document, new string('x', 4096)).Execute();
        Assert.Equal(4096, document.Text.Length);
        receipt.Undo();
        Assert.Equal("", document.Text);
        Assert.Throws<ArgumentOutOfRangeException>(() => new AppendTextCommand(document, new string('x', 4097)));
    }

    [Fact]
    public void WhitespaceAndUnicode_ArePreserved()
    {
        var document = new TextDocument();
        new AppendTextCommand(document, " .NET — học\n").Execute();
        Assert.Equal(" .NET — học\n", document.Text);
    }

    [Fact]
    public void NullCollaborators_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new AppendTextCommand(null!, "A"));
        Assert.Throws<ArgumentNullException>(() => new DirectEditor(null!));
        Assert.Throws<ArgumentNullException>(() => new DelegateButton(null!));
        Assert.Throws<ArgumentNullException>(() => new EditHistory().Execute(null!));
        Assert.Throws<ArgumentNullException>(() => CommandDemo.Run(null!));
    }

    [Fact]
    public void DelegateButton_DefersAndRepeatsAction()
    {
        var document = new TextDocument();
        var button = new DelegateButton(() => document.Append("A"));
        Assert.Equal("", document.Text);
        button.Click();
        button.Click();
        Assert.Equal("AA", document.Text);
    }

    [Fact]
    public void Demo_HasDeterministicOutput()
    {
        using var output = new StringWriter();
        CommandDemo.Run(output);
        Assert.Equal(string.Join(Environment.NewLine,
            "Baseline: Learn .NET", "Command: Learn .NET", "After undo: Learn",
            "Re-executed: Learn .NET", "Delegate: Callback", ""), output.ToString());
    }

    private sealed class CallbackCommand(Action action) : IEditCommand
    {
        public IUndoEdit Execute()
        {
            action();
            throw new InvalidOperationException("This probe must fail before completing.");
        }
    }
}

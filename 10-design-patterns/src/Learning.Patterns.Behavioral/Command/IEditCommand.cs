namespace Learning.Patterns.Behavioral.Command;

/// <summary>Execute atomically in this in-memory lab; return one undo receipt per successful call.</summary>
public interface IEditCommand
{
    IUndoEdit Execute();
}

/// <summary>Undo once, or throw without changing the document if the current state conflicts.</summary>
public interface IUndoEdit
{
    void Undo();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandWrapper
{
    void Undo();
    void Redo();
    void ClearRedo();
    void EraseOldestHistoryEntry(); //new
}

public sealed class CommandWrapper<T> : ICommandWrapper where T : struct //exec args
{
    public CommandType CommandType { get; private set; }

    private readonly ICommand<T> _cmd;

    private readonly List<HistoryEntry> _history = new();

    public CommandWrapper(ICommand<T> cmd, CommandType commandType)
    {
        _cmd = cmd;
        CommandType = commandType;
    }

    public void Execute(T args)
    {
        ClearRedo();
        _history.Add(new(args));
    }

    public void Undo()
    {
        //new 
        //  var undoArgs = _history[_history.Count - 1];
        //  _history.RemoveAt(_history.Count - 1);  //remove after taking
    

        // _cmd.Undo(undoArgs.UndoArgs);
        //  _toRedo.Push(undoArgs.ExecutionArgs);
     }

    public void Redo()
    {
        // var args = _toRedo.Pop();
        // _history.Add(new(args));
    }

    public void ClearRedo()
    {
        // _toRedo.Clear();
    }

    //new 
    public void EraseOldestHistoryEntry()
    {
        _history.RemoveAt(0);
    }

    private readonly struct HistoryEntry
    {
        public readonly T ExecutionArgs;
        //public readonly T1 UndoArgs;

        public HistoryEntry(T executionArgs)
        {   
            ExecutionArgs = executionArgs;
        }
    }
}

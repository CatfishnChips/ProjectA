using System.Collections.Generic;
using UnityEngine;

// Command Invoker
public class CommandInvoker
{
    //all wrappers by CommandType
    private Dictionary<CommandType, ICommandWrapper> _commandWrappers = new();

    #region Command Wrappers

    private readonly CommandWrapper<AttackCommandArgs> _attack;
    private readonly CommandWrapper<MoveCommandArgs> _move;
    private readonly CommandWrapper<SwipeCommandArgs> _swipe;
    private readonly CommandWrapper<TapCommandArgs> _tap;

    #endregion

    //undo and redo
    // private readonly List<CommandType> _undoCommandTypes = new(_maxUndoCount);
    // private readonly Stack<CommandType> _redoCommandTypes = new();

    // private const int _maxUndoCount = 4;    //undo memory limit

    public CommandInvoker()
    {
        // Create Wrappers
        _attack = new(new AttackCommand(), CommandType.Attack);
        _move = new(new MoveCommand(), CommandType.Move);
        _swipe = new(new SwipeCommand(), CommandType.Swipe);
        _tap = new(new TapCommand(), CommandType.Tap);

        // Add wrappers to the dictionary.
        _commandWrappers.Add(CommandType.Attack, _attack);
        _commandWrappers.Add(CommandType.Move, _move);
        _commandWrappers.Add(CommandType.Swipe, _swipe);
        _commandWrappers.Add(CommandType.Tap, _tap);
    }

    #region Execute Methods

    public void ExecuteAttack(AttackCommandArgs args)
    {
        _attack.Execute(args);    //ask wrapper to execute
        // AddToUndo(CommandType.SpawnCube);  //add command type to history stack
        // ClearRedo();
    }

    public void ExecuteMove(MoveCommandArgs args)
    {
        _move.Execute(args);
        // AddToUndo(CommandType.MoveObject);
        // ClearRedo();
    }

    public void ExecuteSwipe(SwipeCommandArgs args)
    {
        _swipe.Execute(args);
    }

    public void ExecuteTap(TapCommandArgs args)
    {
        _tap.Execute(args);
    }

    #endregion

    public void Undo()
    {
        // var commandType = _undoCommandTypes[_undoCommandTypes.Count - 1];  //pop command type from history stack
        // _undoCommandTypes.RemoveAt(_undoCommandTypes.Count - 1);    //remove undone command from history

        // _commandWrappers[commandType].Undo();   //ask wrapper of this command type to undo

        // _redoCommandTypes.Push(commandType);    //push command in redo stack
    }

    public void Redo()
    {
        // var commandType = _redoCommandTypes.Pop();

        // _commandWrappers[commandType].Redo();

        // AddToUndo(commandType);
    }

    private void ClearRedo()
    {
        // _redoCommandTypes.Clear();
        // _spawnCube.ClearRedo();
        // _moveObject.ClearRedo();
    }

    private void AddToUndo(CommandType commandUndo)
    {
        // if (_undoCommandTypes.Count + 1 > _maxUndoCount)
        // {
        //     //erase oldest element in history
        //     _commandWrappers[_undoCommandTypes[0]].EraseOldestHistoryEntry();
        //     _undoCommandTypes.RemoveAt(0);
        // }
        // _undoCommandTypes.Add(commandUndo);
    }
}


// Command Enums
public enum CommandType
{
    Attack,
    Move,
    Swipe,
    Tap
}

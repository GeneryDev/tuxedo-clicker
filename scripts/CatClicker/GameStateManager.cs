using System.Collections.Generic;
using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

public partial class GameStateManager : SingletonNode<GameStateManager>, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    public GameState State = new();

    public double Now;

    public override void _EnterTree()
    {
        base._EnterTree();
        Now = Time.GetUnixTimeFromSystem();
    }

    public override void _Process(double delta)
    {
        Now = Time.GetUnixTimeFromSystem();
        base._Process(delta);
    }

    public override void _Ready()
    {
        State = new GameState()
        {
            UnixTimestampSec = Now,
            Points = 0
        };
        var generatorStates = new List<PointGeneratorState>();
        foreach (var generator in PointGenerators.CollectAll(new()))
        {
            generatorStates.Add(generator.Resource.GetDefaultState());
        }

        State.GeneratorStates = generatorStates.ToArray();
    }

    public void Step()
    {
        State = GetCurrentState();
    }

    public GameState GetCurrentState()
    {
        return State.AdvanceTo(Now);
    }

    public void Click()
    {
        Step();
        State.Points++;
        EmitSignalUpdated();
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public void AddGenerator(StringName generatorId, int count)
    {
        Step();
        for (int i = 0; i < State.GeneratorStates.Length; i++)
        {
            if (State.GeneratorStates[i].GeneratorId != generatorId) continue;
            State.GeneratorStates[i].Count += count;
        }
    }

    public bool WithdrawPoints(decimal amount)
    {
        Step();
        if (State.Points >= amount)
        {
            State.Points -= amount;
            return true;
        }

        return false;
    }
}
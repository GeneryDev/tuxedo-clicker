using System;
using System.Collections.Generic;
using GDF.Data;
using GDF.Util;
using Godot;
using System.Numerics;

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
        BigInteger increment = 1;
        State.Points += increment;
        FinishStateChange();
        MessageChannel.BroadcastMessage("cursor_msg", $"+{GameInterfaceManager.Instance.FormatNumber(increment)}");
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
        FinishStateChange();
    }

    public bool WithdrawPoints(BigInteger amount)
    {
        Step();
        if (State.Points >= amount)
        {
            State.Points -= amount;
            return true;
        }
        FinishStateChange();

        return false;
    }

    public void GainEffect(StringName effectId, double duration)
    {
        Step();
        var newEffects = new ActiveEffectState[(State.ActiveEffectStates?.Length ?? 0) + 1];
        if (State.ActiveEffectStates != null)
            Array.Copy(State.ActiveEffectStates, newEffects, State.ActiveEffectStates.Length);
        newEffects[^1] = new()
        {
            EffectId = effectId,
            RemainingSec = duration
        };
        State.ActiveEffectStates = newEffects;
        FinishStateChange();
    }

    private void FinishStateChange()
    {
        State.ProgressionData.UpdateFromGameState(State);
        MarkDirty();
        EmitSignalUpdated();
    }
}
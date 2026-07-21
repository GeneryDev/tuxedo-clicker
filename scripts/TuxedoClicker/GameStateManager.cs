using System;
using System.Collections.Generic;
using GDF.Data;
using GDF.Util;
using Godot;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;

namespace TuxedoClicker;

public partial class GameStateManager : SingletonNode<GameStateManager>, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    [Signal]
    public delegate void EndingReachedEventHandler();
    
    public GameState State = new();

    public double Now;
    private GameState _currentState;

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
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        // Now is copied directly to UnixTimestampSec in AdvanceTo
        if (_currentState.UnixTimestampSec == Now)
        {
            return _currentState;
        }
        return _currentState = State.AdvanceTo(Now);
    }

    public void Click()
    {
        Step();
        BigInteger increment = 1;
        State.ComputeTotalProductionRate(out var totalRate);
        State.ModifyClickProduction(totalRate, ref increment);
        State.AddClickedPoints(increment);
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
        newEffects[^1] = new(effectId, duration);
        State.ActiveEffectStates = newEffects;
        FinishStateChange();
    }

    public void NotifyBonusItemClick()
    {
        Step();
        State.BonusItemClicks++;
        FinishStateChange();
    }
    private void FinishStateChange()
    {
        State.ProgressionData.UpdateFromGameState(State);
        MarkDirty();
        EmitSignalUpdated();
    }

    public void AddUpgrade(StringName upgradeId)
    {
        Step();
        State.ProgressionData.AddUpgrade(upgradeId);
        FinishStateChange();
    }
    
    public void Reset()
    {
        Instance.LoadState(NewBlankState());
        FinishStateChange();
    }


    [JSImport("test_import")]
    public static partial void TestJsImport();

    [JSExport()]
    public static void TestJsExport()
    {
        GD.Print("hi");
    }
}
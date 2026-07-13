using GDF.Data;
using GDF.Util;
using Godot;

namespace TuxedoClicker;

public struct ActiveEffectStateContext : IDataContext, ICacheableDataContext<ActiveEffectStateContext>
{
    public int ActiveEffectIndex;

    public ActiveEffectStateContext(int index)
    {
        ActiveEffectIndex = index;
    }

    public StringName EffectId => GetState().EffectId;
    public Effect Definition => Effects.FromId(EffectId).Resource;

    public ActiveEffectState GetState()
    {
        var gameState = GetCurrentGameState();
        if (gameState.ActiveEffectStates != null && ActiveEffectIndex < gameState.ActiveEffectStates.Length)
        {
            return gameState.ActiveEffectStates[ActiveEffectIndex];
        }

        return default;
    }
    
    public GameState GetCurrentGameState()
    {
        if (!GameStateManager.InstanceExists) return default;
        return GameStateManager.Instance.GetCurrentState();
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
            case "id":
            case "effect_id":
            {
                output = EffectId;
                return true;
            }
            case "remaining_time":
            {
                output = GetState().RemainingSec;
                return true;
            }
            case "application_duration":
            {
                output = GetState().ApplicationDuration;
                return true;
            }
            case "progress":
            {
                var state = GetState();
                if (state.ApplicationDuration != 0)
                {
                    output = state.RemainingSec / state.ApplicationDuration;
                }
                else
                {
                    output = 0;
                }
                return true;
            }
        }

        return false;
    }

    public bool GetSubContext(string key, string input, ref IDataContext output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "effect":
            case "effect_definition":
            {
                output = Definition;
                return true;
            }
        }

        return false;
    }


    public void ConnectUpdateSignal(Callable callable)
    {
        if (!GameStateManager.InstanceExists) return;
        GameStateManager.Instance.TryConnect(TuxedoClicker.GameStateManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!GameStateManager.InstanceExists) return;
        GameStateManager.Instance.TryDisconnect(TuxedoClicker.GameStateManager.SignalName.Updated, callable);
    }

    public bool EqualsContext(ActiveEffectStateContext otherCtx)
    {
        return this.ActiveEffectIndex == otherCtx.ActiveEffectIndex;
    }

    public bool CanCache() => true;
}
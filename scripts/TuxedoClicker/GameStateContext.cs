using System.Collections.Generic;
using GDF.Data;
using GDF.Data.Static;
using GDF.Util;
using Godot;

namespace TuxedoClicker;

[StaticDataContext("game_state")]
public struct GameStateContext : IDataContext, ICacheableDataContext<GameStateContext>
{
    private GameState GetCurrentState()
    {
        if (!GameStateManager.InstanceExists) return default;
        return GameStateManager.Instance.GetCurrentState();
    }
    
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
            case "points":
            {
                output = (long)GetCurrentState().Points;
                return true;
            }
            case "has_effect":
            {
                output = GetCurrentState().HasEffect(input);
                return true;
            }
            case "is_save_dirty":
            {
                output = GameStateManager.Instance.IsDirty();
                return true;
            }
        }

        return false;
    }

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
            case "points":
            {
                replacement = $"{GameInterfaceManager.Instance.FormatNumber(GetCurrentState().Points)}";
                return true;
            }
            case "points_per_second":
            {
                var rate = GameStateManager.Instance.GetCurrentState()
                    .ComputeTotalProductionRate(out _);

                replacement = rate.ToString();
                return true;
            }
            case "total_points":
            {
                var state = GetCurrentState();
                replacement = $"{GameInterfaceManager.Instance.FormatNumber(state.TotalGeneratedPoints + state.TotalClickedPoints)}";
                return true;
            }
            case "total_manual_points":
            {
                var state = GetCurrentState();
                replacement = $"{GameInterfaceManager.Instance.FormatNumber(state.TotalClickedPoints)}";
                return true;
            }
            case "bonus_item_clicks":
            {
                var state = GetCurrentState();
                replacement = $"{GameInterfaceManager.Instance.FormatNumber(state.BonusItemClicks)}";
                return true;
            }
            case "total_clicks":
            {
                var state = GetCurrentState();
                replacement = $"{GameInterfaceManager.Instance.FormatNumber(state.TotalClicks)}";
                return true;
            }
            case "run_start_time_relative":
            {
                var state = GetCurrentState();
                replacement = $"{GameInterfaceManager.Instance.FormatTime(GameStateManager.Instance.Now - state.ProgressionData.RunStartedUnixTimestamp)}";
                return true;
            }
        }

        return false;
    }

    public bool GetContextCollection(string key, string input, List<IDataContext> output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "active_effects":
            {
                var state = GetCurrentState();
                if (state.ActiveEffectStates != null)
                {
                    for (int i = 0; i < state.ActiveEffectStates.Length; i++)
                    {
                        output.Add(new ActiveEffectStateContext(i).Boxed());
                    }
                }
                return true;
            }
        }

        return false;
    }

    public void ConnectUpdateSignal(Callable callable)
    {
        if (!GameStateManager.InstanceExists) return;
        GameStateManager.Instance.TryConnect(GameStateManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!GameStateManager.InstanceExists) return;
        GameStateManager.Instance.TryDisconnect(GameStateManager.SignalName.Updated, callable);
    }

    public bool EqualsContext(GameStateContext otherCtx)
    {
        return true;
    }

    public bool CanCache() => true;
}
using GDF.Data;
using GDF.Data.Static;
using GDF.Util;
using Godot;

namespace CatClicker;

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
                replacement = $"{GameInterfaceManager.Instance.FormatNumber(GetCurrentState().Points)} pets";
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
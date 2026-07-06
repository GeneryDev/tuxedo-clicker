using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

public partial class PointGeneratorInterface : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    public StringName UpdatedSignalName => SignalName.Updated;
    
    [Export] public string GeneratorId = "";
    
    [Export] public float RefreshInterval = 0.1f;
    private Accumulator _refreshTimer;

    public override void _Process(double delta)
    {
        base._Process(delta);
        _refreshTimer.Add((float)delta);
        var shouldRefresh = false;
        while (_refreshTimer.Consume(RefreshInterval))
        {
            shouldRefresh = true;
        }
        if(shouldRefresh) Refresh();
    }

    private void Refresh()
    {
        EmitSignalUpdated();
    }
    
    private PointGeneratorState GetCurrentState()
    {
        var gameState = GameStateManager.Instance.GetCurrentState();
        foreach (var state in gameState.GeneratorStates)
        {
            if (state.GeneratorId == GeneratorId) return state;
        }

        return default;
    }

    private double GetProgress()
    {
        var state = GetCurrentState();

        return (double)((decimal)state.Phase * state.TotalTickRate);
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "progress":
            {
                output = GetProgress();
                return true;
            }
        }

        return false;
    }
}
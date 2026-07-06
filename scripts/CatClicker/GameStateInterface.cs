using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

public partial class GameStateInterface : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    public StringName UpdatedSignalName => SignalName.Updated;

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

    private GameState GetCurrentState()
    {
        return GameStateManager.Instance.GetCurrentState();
    }
    
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "points":
            {
                output = (double)GetCurrentState().Points;
                return true;
            }
        }

        return false;
    }

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "points":
            {
                replacement = $"{GetCurrentState().Points} pets";
                return true;
            }
        }

        return false;
    }

    public void Click()
    {
        GameStateManager.Instance.Click();
    }
}
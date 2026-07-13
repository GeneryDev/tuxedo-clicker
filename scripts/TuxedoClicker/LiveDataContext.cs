using GDF.Data;
using GDF.Util;
using Godot;

namespace TuxedoClicker;

public partial class LiveDataContext : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    public StringName UpdatedSignalName => TuxedoClicker.LiveDataContext.SignalName.Updated;

    [Export] public Node ParentDataContext;

    [Export] public float RefreshInterval = 0.1f;
    private Accumulator _refreshTimer;

    public IDataContext ParentContext => ParentDataContext as IDataContext;

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

    public override void _EnterTree()
    {
        base._EnterTree();
        ParentContext?.ConnectUpdateSignal(new Callable(this, TuxedoClicker.LiveDataContext.MethodName.Refresh));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ParentContext?.DisconnectUpdateSignal(new Callable(this, TuxedoClicker.LiveDataContext.MethodName.Refresh));
    }
}
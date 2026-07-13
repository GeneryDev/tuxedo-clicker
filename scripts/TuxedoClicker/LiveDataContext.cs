using GDF.Data;
using GDF.Util;
using Godot;
using Godot.Collections;

namespace TuxedoClicker;

public partial class LiveDataContext : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    [Signal]
    public delegate void ContextSignalReceivedEventHandler(StringName signalName, Array args);

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

    public void ReceiveContextSignal(StringName signalName, Array args)
    {
        EmitSignalContextSignalReceived(signalName, args);
    }
    
    public StringName UpdatedSignalName => SignalName.Updated;
    public StringName ContextSignalReceivedSignalName => SignalName.ContextSignalReceived;

    public override void _EnterTree()
    {
        base._EnterTree();
        ParentContext?.ConnectUpdateSignal(new Callable(this, MethodName.Refresh));
        ParentContext?.ConnectContextSignal(new Callable(this, MethodName.ReceiveContextSignal));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ParentContext?.DisconnectUpdateSignal(new Callable(this, MethodName.Refresh));
        ParentContext?.DisconnectContextSignal(new Callable(this, MethodName.ReceiveContextSignal));
    }
}
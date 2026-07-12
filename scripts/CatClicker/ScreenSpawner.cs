using System.Collections.Generic;
using GDF.Data;
using GDF.Logical.Signals;
using GDF.UI;
using GDF.Util;
using Godot;

namespace CatClicker;

[GlobalClass]
public partial class ScreenSpawner : Node
{
    [Export] public PackedScene Template;

    [Export] public Node Context;
    [Export] public Godot.Collections.Dictionary<StringName, NodePath> DataContextsBySlot = new();
    [Export] public SignalStation ConnectSignalStation;

    private readonly List<Screen> _created = new();

    public void Trigger()
    {
        var parent = this;
        
        var screen = Template.GdfInstantiate<Screen>();
        screen.Order = Screen.FindAncestorScreenOrder(this, screen.Order);
        var nodeToEnterTree = screen.ToPlaceholder();
        screen.InjectContext(GetContext());

        if (DataContextsBySlot != null)
        {
            foreach (var (slotId, contextNode) in DataContextsBySlot)
            {
                screen.InjectContext(slotId, this.GetNodeOrNull(contextNode) as IDataContext);
            }
        }

        if (ConnectSignalStation != null)
        {
            screen.ConnectSignalStation(ConnectSignalStation);
        }

        if (!IsInstanceValid(screen) || screen.IsQueuedForDeletion()) return;
        _created.Add(screen);
        
        parent.AddChild(nodeToEnterTree);
        screen.ShowScreen();
    }

    private IDataContext GetContext()
    {
        if (Context is not IDataContext nodeContext) return null;
        return nodeContext;
    }

    public void CloseAll()
    {
        foreach (var screen in _created)
        {
            if(IsInstanceValid(screen)) screen.ForceFadeOutScreen();
        }
        _created.Clear();
    }
}
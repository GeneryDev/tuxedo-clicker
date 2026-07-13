using System.Collections.Generic;
using GDF.Data;
using GDF.Logical.Signals;
using GDF.UI;
using GDF.Util;
using Godot;

namespace TuxedoClicker;

[GlobalClass]
public partial class TooltipSpawner : Node
{
    private const int MousePlayerId = -1;

    [Export] public PackedScene Template;

    [Export(PropertyHint.Range, "0,10,0.1,or_greater,suffix:s")]
    public float Delay = 0;

    [Export] public bool ShowOnHover = true;

    [Export] public Node TooltipContext;
    [Export] public string SubContextQuery;
    [Export] public Godot.Collections.Dictionary<StringName, NodePath> DataContextsBySlot = new();

    [Export] public UserInterfaceComponent UIComponent;
    [Export] public Control TooltipOrigin;
    [Export] public SignalStation ConnectSignalStation;

    private Screen _tooltipScreen;
    private Timer _timer;
    private HashSet<int> _focusedPlayers = new();

    private ParsedDataQuery _queryCache;

    public override void _Ready()
    {
        base._Ready();
        if (UIComponent != null)
        {
            UIComponent.PlayerFocusEntered += OnFocusEntered;
            UIComponent.PlayerFocusExited += OnFocusExited;
            if (ShowOnHover)
            {
                UIComponent.FocusableControl.MouseEntered += OnMouseEntered;
                UIComponent.FocusableControl.MouseExited += OnMouseExited;
            }
        }

        _timer = new Timer()
        {
            Autostart = false,
            OneShot = true,
            ProcessMode = ProcessModeEnum.Always
        };
        AddChild(_timer);
        _timer.Timeout += ShowPopup;
    }

    private void OnMouseEntered()
    {
        OnFocusEntered(MousePlayerId);
    }

    private void OnMouseExited()
    {
        OnFocusExited(MousePlayerId);
    }

    private bool AcceptsPlayer(int playerId)
    {
        if (ShowOnHover && playerId == MousePlayerId) return true;
        if (!UIComponent.GetUserInterface().AcceptsPlayerId(playerId)) return false;
        return true;
    }

    private void OnFocusEntered(int playerId)
    {
        if (!AcceptsPlayer(playerId)) return;

        _focusedPlayers.Add(playerId);
        if (_focusedPlayers.Count > 1) return;

        if (Template == null) return;
        if (Delay <= 0) ShowPopup();
        else _timer.Start(Delay);
    }

    private void OnFocusExited(int playerId)
    {
        _focusedPlayers.Remove(playerId);
        if (_focusedPlayers.Count > 0) return;
        ClosePopup();
    }

    private void ShowPopup()
    {
        if (IsInstanceValid(_tooltipScreen)) return;
        var origin = TooltipOrigin ?? UIComponent.FocusableControl;

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
        
        origin.AddChild(nodeToEnterTree);
        _tooltipScreen = screen;
        screen.ShowScreen();
    }

    private IDataContext GetContext()
    {
        if (TooltipContext is not IDataContext nodeContext) return null;
        if (string.IsNullOrEmpty(SubContextQuery)) return nodeContext;
        else return nodeContext.EvaluateSubContext(SubContextQuery, ref _queryCache);
    }

    private void ClosePopup()
    {
        _timer.Stop();
        _tooltipScreen?.FadeOutScreen();
        _tooltipScreen = null;
    }
}
using GDF.Util;
using Godot;

namespace CatClicker;

[GlobalClass]
public partial class SubViewportScaler : Node
{
    [Export]
    public Vector2I OverrideBaseSize;
    
    [Export(PropertyHint.Range, "0,4,0.01,or_greater")]
    public float Scale
    {
        get => _scale;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_scale == value) return;
            _scale = value;
            Update();
        }
    }
    
    [ExportGroup("Node References")]
    [Export]
    public Control SubViewportContainer
    {
        get => _subViewportContainer;
        set
        {
            if (_subViewportContainer == value) return;
            _subViewportContainer?.TryDisconnect(Control.SignalName.Resized, new Callable(this, MethodName.Update));
            _subViewportContainer = value;
            _subViewportContainer?.TryConnect(Control.SignalName.Resized, new Callable(this, MethodName.Update));
            Update();
        }
    }

    [Export]
    public SubViewport SubViewport
    {
        get => _subViewport;
        set
        {
            if (_subViewport == value) return;
            _subViewport = value;
            Update();
        }
    }

    [Export]
    public CanvasLayer SubViewportCanvas
    {
        get => _subViewportCanvas;
        set
        {
            if (_subViewportCanvas == value) return;
            _subViewportCanvas = value;
            Update();
        }
    }

    private Control _subViewportContainer;
    private SubViewport _subViewport;
    private CanvasLayer _subViewportCanvas;
    private float _scale = 1.0f;

    public override void _EnterTree()
    {
        GetWindow()?.TryConnect(Viewport.SignalName.SizeChanged, new Callable(this, MethodName.Update));
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GetWindow()?.TryDisconnect(Viewport.SignalName.SizeChanged, new Callable(this, MethodName.Update));
    }

    public void Update()
    {
        if (_subViewportContainer == null || _subViewport == null || GetWindow() is not {} window) return;

        var size = _subViewportContainer.Size;
        if (OverrideBaseSize != default) size = OverrideBaseSize;
        var currentScale = _scale * window.ContentScaleFactor;
        size *= currentScale;

        size = size.Max(1);

        _subViewport.Size = (Vector2I)size;
        _subViewportCanvas?.SetTransform(Transform2D.Identity.Scaled(new Vector2(1, 1) * currentScale)
            .Translated(size * 0.5f));
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == NotificationPredelete)
        {
            _subViewportContainer?.TryDisconnect(Control.SignalName.Resized, new Callable(this, MethodName.Update));
        }
    }
}
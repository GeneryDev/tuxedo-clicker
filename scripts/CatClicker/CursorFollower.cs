using Godot;

namespace CatClicker;

public partial class CursorFollower : Control
{
    public override void _Process(double delta)
    {
        var window = GetWindow();
        base._Process(delta);
        this.Position = window.GetFinalTransform().AffineInverse() * window.GetMousePosition();
    }
}
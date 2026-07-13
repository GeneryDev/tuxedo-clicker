using GDF.Util;
using Godot;

namespace TuxedoClicker;

public partial class GameEntryPoint : Node
{
    [Export] public PackedScene MainScene;
    
    public override void _Ready()
    {
        var instantiated = MainScene?.GdfInstantiate();
        if (instantiated != null)
        {
            GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToNode, instantiated);
        }
    }
}
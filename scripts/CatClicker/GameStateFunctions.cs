using Godot;

namespace CatClicker;

public partial class GameStateFunctions : Node
{
    public void Click()
    {
        GameStateManager.Instance.Click();
    }

    public void GainEffect(StringName effectId, double duration)
    {
        GameStateManager.Instance.GainEffect(effectId, duration);
    }
}
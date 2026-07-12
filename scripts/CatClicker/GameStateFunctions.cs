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

    public void Save()
    {
        GameStateManager.Instance.Save();
    }

    public void Autosave()
    {
        GameStateManager.Instance.Autosave();
    }

    public void Load()
    {
        GameStateManager.Instance.Load();
    }

    public void Reset()
    {
        GameStateManager.Instance.Reset();
    }
}
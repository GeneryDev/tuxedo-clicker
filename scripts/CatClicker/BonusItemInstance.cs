using Godot;

namespace CatClicker;

public partial class BonusItemInstance : Node
{
    [Signal]
    public delegate void OutcomeStartedEventHandler(string name);

    public void Decide()
    {
        Empty();
    }

    private void Empty()
    {
        GameStateManager.Instance.GainEffect("frenzy", 10);
        EmitSignalOutcomeStarted("empty");
    }
}
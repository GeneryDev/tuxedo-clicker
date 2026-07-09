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
        MessageChannel.BroadcastMessage("effect_msg", "[b]You found an empty cardboard box![/b]\n(x7 production multiplier for 10s)");
    }
}
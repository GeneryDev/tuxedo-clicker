using Godot;

namespace CatClicker;

public partial class BonusItemInstance : Node
{
    [Signal]
    public delegate void OutcomeStartedEventHandler(string name);

    public void Decide()
    {
        GameStateManager.Instance.NotifyBonusItemClick();
        Empty();
    }

    private void Empty()
    {
        var duration = 15;
        if (GameStateManager.Instance.State.ProgressionData.HasUpgrade("box_tier_2"))
        {
            duration *= 2;
        }
        GameStateManager.Instance.GainEffect("frenzy", duration);
        EmitSignalOutcomeStarted("empty");
        MessageChannel.BroadcastMessage("effect_msg", $"[b]You found an empty cardboard box![/b]\n(x9 production multiplier for {duration}s)");
    }
}
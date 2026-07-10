using Godot;
using System.Numerics;

namespace CatClicker;

public partial class UpgradeFunctions : Node
{
    [Export] public StringName UpgradeId = "";

    public void Purchase()
    {
        var state = new UpgradeStateContext(UpgradeId);
        if (!state.CanAffordPurchase()) return;
        if (state.IsActive()) return;
        var cost = state.GetPurchaseCost();
        GameStateManager.Instance.AddUpgrade(UpgradeId);
        GameStateManager.Instance.WithdrawPoints(cost);
    }
}
using Godot;

namespace TuxedoClicker;

public partial class UpgradeFunctions : Node
{
    [Signal]
    public delegate void PurchaseSuccessEventHandler();
    [Signal]
    public delegate void PurchaseFailedEventHandler();
    
    [Export] public StringName UpgradeId = "";

    public void Purchase()
    {
        var state = new UpgradeStateContext(UpgradeId);
        if (!state.CanAffordPurchase())
        {
            EmitSignalPurchaseFailed();
            return;
        }
        if (state.IsActive()) return;
        var cost = state.GetPurchaseCost();
        GameStateManager.Instance.AddUpgrade(UpgradeId);
        GameStateManager.Instance.WithdrawPoints(cost);
        EmitSignalPurchaseSuccess();
    }
}
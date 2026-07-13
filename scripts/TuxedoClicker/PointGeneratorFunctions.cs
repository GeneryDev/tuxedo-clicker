using System.Numerics;
using Godot;

namespace TuxedoClicker;

public partial class PointGeneratorFunctions : Node
{
    [Signal]
    public delegate void PurchaseSuccessEventHandler();
    [Signal]
    public delegate void PurchaseFailedEventHandler();
    
    [Export] public StringName GeneratorId = "";

    public void Purchase()
    {
        var state = new PointGeneratorStateContext(GeneratorId);
        if (!state.CanAffordSinglePurchase())
        {
            EmitSignalPurchaseFailed();
            return;
        }
        int countToPurchase = state.CanAffordBulkPurchase() ? GameInterfaceManager.Instance.GetBulkPurchaseAmount() : state.GetAffordableBulkCount();
        int currentCount = state.GetCount();
        BigInteger totalCost = state.Definition.GetPointCostForLevels(currentCount, currentCount + countToPurchase);
        GameStateManager.Instance.AddGenerator(GeneratorId, countToPurchase);
        GameStateManager.Instance.WithdrawPoints(totalCost);
        EmitSignalPurchaseSuccess();
    }
}
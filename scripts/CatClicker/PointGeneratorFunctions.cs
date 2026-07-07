using Godot;
using System.Numerics;

namespace CatClicker;

public partial class PointGeneratorFunctions : Node
{
    [Export] public StringName GeneratorId = "";

    public void Purchase()
    {
        var state = new PointGeneratorStateContext(GeneratorId);
        if (!state.CanAffordSinglePurchase()) return;
        int countToPurchase = state.CanAffordBulkPurchase() ? GameInterfaceManager.Instance.GetBulkPurchaseAmount() : state.GetAffordableBulkCount();
        int currentCount = state.GetCount();
        BigInteger totalCost = state.Definition.GetPointCostForLevels(currentCount, currentCount + countToPurchase);
        GameStateManager.Instance.AddGenerator(GeneratorId, countToPurchase);
        GameStateManager.Instance.WithdrawPoints(totalCost);
    }
}
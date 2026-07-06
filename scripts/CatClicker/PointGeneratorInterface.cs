using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

public partial class PointGeneratorInterface : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();

    public StringName UpdatedSignalName => SignalName.Updated;
    
    [Export] public StringName GeneratorId = "";
    
    [Export] public float RefreshInterval = 0.1f;
    private Accumulator _refreshTimer;

    public override void _Process(double delta)
    {
        base._Process(delta);
        _refreshTimer.Add((float)delta);
        var shouldRefresh = false;
        while (_refreshTimer.Consume(RefreshInterval))
        {
            shouldRefresh = true;
        }
        if(shouldRefresh) Refresh();
    }

    public PointGenerator Definition => PointGenerators.FromId(GeneratorId).Resource;

    private void Refresh()
    {
        EmitSignalUpdated();
    }
    
    private PointGeneratorState GetCurrentState()
    {
        var gameState = GameStateManager.Instance.GetCurrentState();
        foreach (var state in gameState.GeneratorStates)
        {
            if (state.GeneratorId == GeneratorId) return state;
        }

        return default;
    }

    private double GetProgress()
    {
        var state = GetCurrentState();

        return (double)((decimal)state.Phase * state.TotalTickRate);
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "progress":
            {
                output = GetProgress();
                return true;
            }
            case "count":
            {
                output = GetCurrentState().Count;
                return true;
            }
            case "purchase_cost":
            case "bulk_purchase_cost":
            {
                output = (long)GetBulkPurchaseCost();
                return true;
            }
            case "single_purchase_cost":
            {
                output = (long)GetSinglePurchaseCost();
                return true;
            }
            case "can_afford":
            case "can_afford_bulk":
            {
                output = CanAffordBulkPurchase();
                return true;
            }
            case "can_afford_single":
            {
                output = CanAffordSinglePurchase();
                return true;
            }
        }

        return false;
    }

    public decimal GetSinglePurchaseCost()
    {
        return Definition.GetPointCostForLevel(GetCurrentState().Count);
    }

    public decimal GetBulkPurchaseCost()
    {
        int currentCount = GetCurrentState().Count;
        Definition.TestMultiLevelCostFunction(currentCount, currentCount + GameInterfaceManager.Instance.GetBulkPurchaseAmount());
        return Definition.GetPointCostForLevels(currentCount, currentCount + GameInterfaceManager.Instance.GetBulkPurchaseAmount());
    }

    public bool CanAffordSinglePurchase()
    {
        return GameStateManager.Instance.GetCurrentState().Points >= GetSinglePurchaseCost();
    }

    public bool CanAffordBulkPurchase()
    {
        return GameStateManager.Instance.GetCurrentState().Points >= GetBulkPurchaseCost();
    }

    public void Purchase()
    {
        if (!CanAffordSinglePurchase()) return;
        int countToPurchase = CanAffordBulkPurchase() ? GameInterfaceManager.Instance.GetBulkPurchaseAmount() : GetAffordableBulkCount();
        int currentCount = GetCurrentState().Count;
        decimal totalCost = Definition.GetPointCostForLevels(currentCount, currentCount + countToPurchase);
        Definition.TestMultiLevelCostFunction(currentCount, currentCount + countToPurchase);
        GameStateManager.Instance.AddGenerator(GeneratorId, countToPurchase);
        GameStateManager.Instance.WithdrawPoints(totalCost);
    }

    private int GetAffordableBulkCount()
    {
        decimal currentPoints = GameStateManager.Instance.GetCurrentState().Points;
        int currentCount = GetCurrentState().Count;
        int purchaseCount = GameInterfaceManager.Instance.GetBulkPurchaseAmount();
        while (purchaseCount > 0 && currentPoints < Definition.GetPointCostForLevels(currentCount, currentCount + purchaseCount))
        {
            purchaseCount--;
        }

        return purchaseCount;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        GameStateManager.Instance.Updated += EmitSignalUpdated;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameStateManager.Instance.Updated -= EmitSignalUpdated;
    }
}
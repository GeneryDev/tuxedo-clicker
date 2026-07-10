using GDF.Data;
using GDF.Data.Static;
using Godot;
using System.Numerics;
using GDF.Util;

namespace CatClicker;

[StaticDataContext("point_generator_state")]
public struct PointGeneratorStateContext : IDataContext, ICacheableDataContext<PointGeneratorStateContext>
{
    public StringName GeneratorId;

    public PointGeneratorStateContext(StringName generatorId)
    {
        GeneratorId = generatorId;
    }

    public PointGenerator Definition => PointGenerators.FromId(GeneratorId).Resource;
    
    public PointGeneratorState GetCurrentState()
    {
        if (!GameStateManager.InstanceExists) return default;
        var gameState = GameStateManager.Instance.GetCurrentState();
        if (gameState.GeneratorStates != null)
        {
            foreach (var state in gameState.GeneratorStates)
            {
                if (state.GeneratorId == GeneratorId) return state;
            }
        }

        return default;
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
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

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
            case "purchase_cost":
            case "bulk_purchase_cost":
            {
                replacement = GameInterfaceManager.Instance.FormatNumber(GetBulkPurchaseCost());
                return true;
            }
            case "single_purchase_cost":
            {
                replacement = GameInterfaceManager.Instance.FormatNumber(GetSinglePurchaseCost());
                return true;
            }
        }

        return false;
    }
    
    public int GetCount()
    {
        return GetCurrentState().Count;
    }

    public BigInteger GetSinglePurchaseCost()
    {
        return Definition.GetPointCostForLevel(GetCurrentState().Count);
    }

    public BigInteger GetBulkPurchaseCost()
    {
        int currentCount = GetCurrentState().Count;
        Definition.TestMultiLevelCostFunction(currentCount, currentCount + GameInterfaceManager.Instance.GetBulkPurchaseAmount());
        return Definition.GetPointCostForLevels(currentCount, currentCount + GameInterfaceManager.Instance.GetBulkPurchaseAmount());
    }

    public bool CanAffordSinglePurchase()
    {
        if (!GameStateManager.InstanceExists) return false;
        return GameStateManager.Instance.GetCurrentState().Points >= GetSinglePurchaseCost();
    }

    public bool CanAffordBulkPurchase()
    {
        if (!GameStateManager.InstanceExists) return false;
        return GameStateManager.Instance.GetCurrentState().Points >= GetBulkPurchaseCost();
    }

    public int GetAffordableBulkCount()
    {
        var state = new PointGeneratorStateContext(GeneratorId);
        BigInteger currentPoints = GameStateManager.Instance.GetCurrentState().Points;
        int currentCount = state.GetCount();
        int purchaseCount = GameInterfaceManager.Instance.GetBulkPurchaseAmount();
        while (purchaseCount > 0 && currentPoints < state.Definition.GetPointCostForLevels(currentCount, currentCount + purchaseCount))
        {
            purchaseCount--;
        }

        return purchaseCount;
    }

    public void ConnectUpdateSignal(Callable callable)
    {
        if (!GameStateManager.InstanceExists) return;
        GameStateManager.Instance.TryConnect(GameStateManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!GameStateManager.InstanceExists) return;
        GameStateManager.Instance.TryDisconnect(GameStateManager.SignalName.Updated, callable);
    }

    public bool EqualsContext(PointGeneratorStateContext otherCtx)
    {
        return this.GeneratorId == otherCtx.GeneratorId;
    }

    public bool CanCache() => true;
}
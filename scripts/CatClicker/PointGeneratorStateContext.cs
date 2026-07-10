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
    
    public GameState GetCurrentGameState()
    {
        if (!GameStateManager.InstanceExists) return default;
        return GameStateManager.Instance.GetCurrentState();
    }
    public PointGeneratorState GetCurrentGeneratorState()
    {
        if (!GameStateManager.InstanceExists) return default;
        return GetCurrentGameState().GetGeneratorState(GeneratorId);
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
            case "count":
            {
                output = GetCurrentGeneratorState().Count;
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
            case "ever_owned":
            {
                output = (GetCurrentGameState().ProgressionData?.GetMaxOwnedGeneratorCount(GeneratorId) ?? 0) > 0;
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
            case "points_per_second":
            {
                var rate = GetCurrentGameState()
                    .ComputeGeneratorProductionRate(GeneratorId, out _, out _);

                replacement = rate.ToString();
                return true;
            }
            case "production_percentage":
            {
                var currentState = GetCurrentGameState();
                currentState.ComputeGeneratorProductionRate(GeneratorId, out decimal tickRate, out var pointsPerTick);
                currentState.ComputeTotalProductionRate(out decimal totalRate);
                decimal percentage = 0;
                if (totalRate != 0)
                {
                    percentage = (tickRate * pointsPerTick) / totalRate * 100;
                }
                replacement = percentage.ToString("N1");
                return true;
            }
        }

        return false;
    }
    
    public int GetCount()
    {
        return GetCurrentGeneratorState().Count;
    }

    public BigInteger GetSinglePurchaseCost()
    {
        return Definition.GetPointCostForLevel(GetCurrentGeneratorState().Count);
    }

    public BigInteger GetBulkPurchaseCost()
    {
        int currentCount = GetCurrentGeneratorState().Count;
        Definition.TestMultiLevelCostFunction(currentCount, currentCount + GameInterfaceManager.Instance.GetBulkPurchaseAmount());
        return Definition.GetPointCostForLevels(currentCount, currentCount + GameInterfaceManager.Instance.GetBulkPurchaseAmount());
    }

    public bool CanAffordSinglePurchase()
    {
        if (!GameStateManager.InstanceExists) return false;
        return GetCurrentGameState().Points >= GetSinglePurchaseCost();
    }

    public bool CanAffordBulkPurchase()
    {
        if (!GameStateManager.InstanceExists) return false;
        return GetCurrentGameState().Points >= GetBulkPurchaseCost();
    }

    public int GetAffordableBulkCount()
    {
        var state = new PointGeneratorStateContext(GeneratorId);
        var currentPoints = GetCurrentGameState().Points;
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
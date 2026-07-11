using GDF.Data;
using GDF.Data.Static;
using Godot;
using System.Numerics;
using GDF.Util;

namespace CatClicker;

[StaticDataContext("upgrade_state")]
public struct UpgradeStateContext : IDataContext, ICacheableDataContext<UpgradeStateContext>
{
    public StringName UpgradeId;

    public UpgradeStateContext(StringName upgradeId)
    {
        UpgradeId = upgradeId;
    }

    public Upgrade Definition => Upgrades.FromId(UpgradeId).Resource;
    
    public GameState GetCurrentGameState()
    {
        if (!GameStateManager.InstanceExists) return default;
        return GameStateManager.Instance.GetCurrentState();
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!GameStateManager.InstanceExists) return false;
        switch (key)
        {
            case "purchase_cost":
            {
                output = (long)GetPurchaseCost();
                return true;
            }
            case "can_afford":
            {
                output = CanAffordPurchase();
                return true;
            }
            case "active":
            case "is_active":
            {
                output = IsActive();
                return true;
            }
            case "unlocked":
            case "is_unlocked":
            {
                output = IsUnlocked();
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
            {
                replacement = GameInterfaceManager.Instance.FormatNumber(GetPurchaseCost());
                return true;
            }
        }

        return false;
    }

    public bool IsActive()
    {
        return GetCurrentGameState().ProgressionData?.HasUpgrade(UpgradeId) ?? false;
    }

    public bool IsUnlocked()
    {
        var def = Definition;
        if (!string.IsNullOrEmpty(def.AffectedGeneratorId) && def.RequireGeneratorCount is var requiredCount and > 0)
        {
            if (new PointGeneratorStateContext(def.AffectedGeneratorId).GetCount() < requiredCount) return false;
        }

        if (def.RequireTotalGeneratedPoints != 0)
        {
            if (GameStateManager.Instance.GetCurrentState().TotalGeneratedPoints <
                def.RequireTotalGeneratedPoints) return false;
        }

        if (def.RequireTotalClickedPoints != 0)
        {
            if (GameStateManager.Instance.GetCurrentState().TotalClickedPoints <
                def.RequireTotalClickedPoints) return false;
        }

        if (def.RequireTotalClicks != 0)
        {
            if (GameStateManager.Instance.GetCurrentState().TotalClicks <
                def.RequireTotalClicks) return false;
        }

        if (def.RequireBonusItemClicks != 0)
        {
            if (GameStateManager.Instance.GetCurrentState().BonusItemClicks <
                def.RequireBonusItemClicks) return false;
        }
        return true;
    }
    
    public BigInteger GetPurchaseCost()
    {
        return Definition.BaseCost;
    }

    public bool CanAffordPurchase()
    {
        if (!GameStateManager.InstanceExists) return false;
        return GetCurrentGameState().Points >= GetPurchaseCost();
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

    public bool EqualsContext(UpgradeStateContext otherCtx)
    {
        return this.UpgradeId == otherCtx.UpgradeId;
    }

    public bool CanCache() => true;
}
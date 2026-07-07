using GDF.Data;
using GDF.Data.Static;
using GDF.Util;
using Godot;

namespace CatClicker;

[StaticDataContext("game_interface")]
public struct GameInterfaceContext : IDataContext, ICacheableDataContext<GameInterfaceContext>
{
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!GameInterfaceManager.InstanceExists) return false;
        switch (key)
        {
            case "bulk_purchase_amount":
            {
                output = GameInterfaceManager.Instance.GetBulkPurchaseAmount();
                return true;
            }
        }

        return false;
    }

    public void ConnectUpdateSignal(Callable callable)
    {
        if (!GameInterfaceManager.InstanceExists) return;
        GameInterfaceManager.Instance.TryConnect(GameInterfaceManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!GameInterfaceManager.InstanceExists) return;
        GameInterfaceManager.Instance.TryDisconnect(GameInterfaceManager.SignalName.Updated, callable);
    }

    public bool EqualsContext(GameInterfaceContext otherCtx)
    {
        return true;
    }

    public bool CanCache() => true;
}
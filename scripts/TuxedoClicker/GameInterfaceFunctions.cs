using GDF.Data;
using Godot;

namespace TuxedoClicker;

public partial class GameInterfaceFunctions : Node, IDataContext
{
    public void SetBulkPurchaseAmount(int amount)
    {
        GameInterfaceManager.Instance.SetBulkPurchaseAmount(amount);
    }
}
using GDF.Data;
using Godot;

namespace CatClicker;

public partial class GameInterfaceFunctions : Node, IDataContext
{
    public void SetBulkPurchaseAmount(int amount)
    {
        GameInterfaceManager.Instance.SetBulkPurchaseAmount(amount);
    }
}
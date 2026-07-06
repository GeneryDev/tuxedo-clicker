using GDF.Data;
using Godot;

namespace CatClicker;

public partial class GameInterface : Node, IDataContext
{
    public void SetBulkPurchaseAmount(int amount)
    {
        GameInterfaceManager.Instance.SetBulkPurchaseAmount(amount);
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
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
}
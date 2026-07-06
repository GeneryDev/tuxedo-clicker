using GDF.Util;
using Godot;

namespace CatClicker;

public partial class GameInterfaceManager : SingletonNode<GameInterfaceManager>
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    private int _bulkPurchaseAmount = 1;
    
    public int GetBulkPurchaseAmount()
    {
        return _bulkPurchaseAmount;
    }
    public void SetBulkPurchaseAmount(int amount)
    {
        _bulkPurchaseAmount = amount;
        EmitSignalUpdated();
    }

    private void Update()
    {
        EmitSignalUpdated();
    }
}
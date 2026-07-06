using GDF.Data;
using Godot;

namespace CatClicker;

public partial class BulkStoreButton : Button, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    [Export] public int Amount = 1;

    public override void _EnterTree()
    {
        base._EnterTree();
        GameInterfaceManager.Instance.Updated += OnInterfaceUpdated;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameInterfaceManager.Instance.Updated -= OnInterfaceUpdated;
    }

    private void OnInterfaceUpdated()
    {
        EmitSignalUpdated();
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "selected":
            {
                output = GameInterfaceManager.Instance.GetBulkPurchaseAmount() == Amount;
                return true;
            }
            case "amount":
            {
                output = Amount;
                return true;
            }
        }

        return false;
    }

    public void Select()
    {
        GameInterfaceManager.Instance.SetBulkPurchaseAmount(Amount);
    }
}
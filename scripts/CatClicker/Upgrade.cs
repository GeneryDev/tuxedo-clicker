using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

[Tool]
[GlobalClass]
public partial class Upgrade : Resource, IProductionModifier, IDataContext
{
    [Export] public string DisplayName = "";
    [Export(PropertyHint.MultilineText)] public string Description = "";
    [Export(PropertyHint.MultilineText)] public string FlavorText = "";
    
    [Export] public StringName AffectedGeneratorId = "";

    [ExportGroup("Cost")]
    [Export] public long CustomBaseCost = 0;
    [Export] public float BaseCostAsFactorOfGeneratorBaseCost = 0;

    [ExportGroup("Unlock Condition")]
    [Export] public int RequireGeneratorCount = 0;

    [ExportGroup("Modifies Production Rate")] [Export(PropertyHint.GroupEnable)]
    public bool ModifiesProductionRate = false;

    [Export] public double ProductionRateMultiplier = 1.0f;

    public long BaseCost => GetBaseCost();
    
    public Upgrades.Descriptor Descriptor => Upgrades.From(this);

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "id":
            {
                output = Descriptor.Id;
                return true;
            }
            case "name":
            {
                output = DisplayName;
                return true;
            }
            case "description":
            {
                output = Description;
                return true;
            }
            case "flavor_text":
            {
                output = FlavorText;
                return true;
            }
            case "icon":
            {
                output = default;
                return true;
            }
            case "base_cost":
            {
                output = GetBaseCost();
                return true;
            }
        }

        return false;
    }

    public bool GetSubContext(string key, string input, ref IDataContext output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "state":
            case "state_context":
            {
                output = new UpgradeStateContext(Descriptor.Id).Boxed();
                return true;
            }
        }

        return false;
    }

    public bool ModifyGeneratorProductionRate(PointGeneratorState generator, ref decimal rate)
    {
        if (!ModifiesProductionRate) return false;
        if (!AffectedGeneratorId.IsNullOrEmpty())
        {
            if (generator.GeneratorId != AffectedGeneratorId) return false;
        }

        rate *= (decimal)ProductionRateMultiplier;
        return true;
    }

    private long GetBaseCost()
    {
        if (BaseCostAsFactorOfGeneratorBaseCost != 0 && !string.IsNullOrEmpty(AffectedGeneratorId))
        {
            var generatorData = PointGenerators.FromId(AffectedGeneratorId).Resource;
            if (generatorData != null)
            {
                return (long)(generatorData.BaseCost * BaseCostAsFactorOfGeneratorBaseCost);
            }
        }
        return CustomBaseCost;
    }

    [ExportGroup("")]
    [ExportToolButton("Setup as tiered generator upgrade")]
    private Callable ButtonSetupAsTieredGeneratorUpgrade => new Callable(this, MethodName.SetupAsTieredGeneratorUpgrade);

    public void SetupAsTieredGeneratorUpgrade()
    {
        string filename = this.ResourcePath.GetBaseName().GetFile();
        int tier = int.Parse(filename[(filename.LastIndexOf('_') + 1)..]);
        BaseCostAsFactorOfGeneratorBaseCost = new int[] {10, 10*5, 10*5*10}[tier - 1];
        RequireGeneratorCount = new int[] {1, 5, 25}[tier - 1];
    }
}
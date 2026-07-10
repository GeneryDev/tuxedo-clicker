using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

[Tool]
[GlobalClass]
public partial class Upgrade : Resource, IProductionModifier
{
    [Export] public string DisplayName = "";
    [Export(PropertyHint.MultilineText)] public string Description = "";
    [Export(PropertyHint.MultilineText)] public string FlavorText = "";

    [ExportGroup("Modifies Production Rate")] [Export(PropertyHint.GroupEnable)]
    public bool ModifiesProductionRate = false;

    [Export] public double ProductionRateMultiplier = 1.0f;
    [Export] public StringName ModifiedGeneratorId = "";
    
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
        }

        return false;
    }

    public bool ModifyGeneratorProductionRate(PointGeneratorState generator, ref decimal rate)
    {
        if (!ModifiesProductionRate) return false;
        if (!ModifiedGeneratorId.IsNullOrEmpty())
        {
            if (generator.GeneratorId != ModifiedGeneratorId) return false;
        }

        rate *= (decimal)ProductionRateMultiplier;
        return true;
    }
}
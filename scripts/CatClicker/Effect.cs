using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

[Tool]
[GlobalClass]
public partial class Effect : Resource, IProductionModifier
{
    [Export] public string DisplayName;

    [ExportGroup("Modifies Production Rate")] [Export(PropertyHint.GroupEnable)]
    public bool ModifiesProductionRate = false;

    [Export] public double ProductionRateMultiplier = 1.0f;
    [Export] public StringName ModifiedGeneratorId = "";
    
    public Effects.Descriptor Descriptor => Effects.From(this);

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
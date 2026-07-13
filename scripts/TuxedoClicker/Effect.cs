using System.Numerics;
using GDF.Data;
using GDF.Util;
using Godot;

namespace TuxedoClicker;

[Tool]
[GlobalClass]
public partial class Effect : Resource, IProductionModifier, IDataContext
{
    [Export] public string DisplayName;

    [Export] public Texture2D Icon;
    [Export] public Color IconModulate = Colors.White;

    [ExportGroup("Modifies Production Rate")] [Export(PropertyHint.GroupEnable)]
    public bool ModifiesProductionRate = false;

    [Export] public double ProductionRateMultiplier = 1.0f;
    [Export] public StringName AffectedGeneratorId = "";

    [ExportGroup("Modifies Click Production")] [Export(PropertyHint.GroupEnable)]
    public bool ModifiesClickProduction = false;

    [Export] public double ClickProductionFromTotalProduction = 0.0f;
    
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
            case "icon":
            {
                output = Icon;
                return true;
            }
            case "icon_modulate":
            {
                output = IconModulate;
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

    public bool ModifyClickProduction(decimal totalProductionRate, ref BigInteger pointGain)
    {
        if (!ModifiesClickProduction) return false;
        pointGain += new BigInteger((int)(totalProductionRate*(decimal)ClickProductionFromTotalProduction));
        return true;
    }
}
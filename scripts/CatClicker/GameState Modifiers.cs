using System.Numerics;

namespace CatClicker;

public partial struct GameState : IProductionModifier
{
    public bool ModifyGeneratorProductionRate(PointGeneratorState generator, ref decimal rate)
    {
        var any = false;
        if (ActiveEffectStates != null)
        {
            foreach (var state in ActiveEffectStates)
            {
                if (Effects.FromId(state.EffectId).Resource?.ModifyGeneratorProductionRate(generator, ref rate) ?? false)
                    any = true;
            }
        }

        if (ProgressionData?.ActiveUpgrades != null)
        {
            foreach (var upgradeId in ProgressionData.ActiveUpgrades)
            {
                if (Upgrades.FromId(upgradeId).Resource?.ModifyGeneratorProductionRate(generator, ref rate) ?? false)
                    any = true;
            }
        }

        return any;
    }

    public bool ModifyClickProduction(decimal totalProductionRate, ref BigInteger pointGain)
    {
        var any = false;
        if (ActiveEffectStates != null)
        {
            foreach (var state in ActiveEffectStates)
            {
                if (Effects.FromId(state.EffectId).Resource?.ModifyClickProduction(totalProductionRate, ref pointGain) ?? false)
                    any = true;
            }
        }

        if (ProgressionData?.ActiveUpgrades != null)
        {
            foreach (var upgradeId in ProgressionData.ActiveUpgrades)
            {
                if (Upgrades.FromId(upgradeId).Resource?.ModifyClickProduction(totalProductionRate, ref pointGain) ?? false)
                    any = true;
            }
        }

        return any;
    }
}
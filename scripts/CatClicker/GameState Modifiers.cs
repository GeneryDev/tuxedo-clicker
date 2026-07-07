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

        return any;
    }
}
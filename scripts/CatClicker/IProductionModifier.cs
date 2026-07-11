using System.Numerics;

namespace CatClicker;

public interface IProductionModifier
{
    public bool ModifyGeneratorProductionRate(PointGeneratorState generator, ref decimal rate)
    {
        return false;
    }
    public bool ModifyClickProduction(decimal totalProductionRate, ref BigInteger pointGain)
    {
        return false;
    }
}
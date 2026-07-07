namespace CatClicker;

public interface IProductionModifier
{
    public bool ModifyGeneratorProductionRate(PointGeneratorState generator, ref decimal rate)
    {
        return false;
    }
}
namespace CatClicker;

public struct PointGeneratorState
{
    public string GeneratorId;
    public decimal SingleTickRate;
    public int Count;
    public double Phase;
    public decimal PointsPerTick;

    public decimal TotalTickRate => SingleTickRate * Count;
}
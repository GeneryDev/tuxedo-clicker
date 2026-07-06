using Godot;

namespace CatClicker;

public struct PointGeneratorState
{
    public StringName GeneratorId;
    public decimal SingleTickRate;
    public int Count;
    public double Phase;
    public decimal PointsPerTick;

    public decimal TotalTickRate => SingleTickRate * Count;
}
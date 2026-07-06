using System.Numerics;
using Godot;

namespace CatClicker;

public struct PointGeneratorState
{
    public StringName GeneratorId;
    public decimal SingleTickRate;
    public int Count;
    public double Phase;
    public BigInteger PointsPerTick;

    public decimal TotalTickRate => SingleTickRate * Count;
}
using System;
using System.Numerics;
using GDF.Data;
using Godot;

namespace CatClicker;

[Tool]
[GlobalClass]
public partial class PointGenerator : Resource, IDataContext
{
    [Export] public string DisplayName;

    [ExportGroup("Point Generation")]
    [Export] public double TickRate = 1;
    [Export] public long PointsPerTick = 1;

    [ExportGroup("Cost")]
    [Export] public long BaseCost = 1;
    [Export] public double CostScalingPerLevel = 1.4f;
    
    public PointGenerators.Descriptor Descriptor => PointGenerators.From(this);

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
            case "base_cost":
            {
                output = BaseCost;
                return true;
            }
        }

        return false;
    }

    public BigInteger GetPointCostForLevel(int level)
    {
        return new BigInteger(Math.Floor(GetPrecisePointCostForLevel(level)));
    }

    public decimal GetPrecisePointCostForLevel(int level)
    {
        return BaseCost * (decimal)Mathf.Pow(CostScalingPerLevel, level);
    }

    public BigInteger GetPointCostForLevels(int fromLevel, int toLevel) // inclusive, exclusive
    {
        return GetCumulativePointCostToLevel(toLevel - 1) - GetCumulativePointCostToLevel(fromLevel-1);
    }

    public BigInteger GetCumulativePointCostToLevel(int level)
    {
        return new BigInteger(Math.Floor(BaseCost *
            (Mathf.Pow(CostScalingPerLevel, level + 1) - 1) / (CostScalingPerLevel - 1)));
    }

    public void TestMultiLevelCostFunction(int fromLevel, int toLevel) // inclusive, exclusive
    {
        // var bruteForceSum = 0M;
        // for (int i = fromLevel; i < toLevel; i++) bruteForceSum += GetPrecisePointCostForLevel(i);
        // var bruteForceSolution = new BigInteger(Math.Round(bruteForceSum));
        //
        // BigInteger fastSolution = GetPointCostForLevels(fromLevel, toLevel);
        // if (BigInteger.Abs(bruteForceSolution - fastSolution) > 1)
        // {
        //     GD.PrintErr($"Multi level cost function failed for inputs {fromLevel}, {toLevel}. Expected: {bruteForceSolution}; Actual: {fastSolution}");
        // }
    }
}
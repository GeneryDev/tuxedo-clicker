using System;
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
    [Export] public double PointsPerTick = 1;

    [ExportGroup("Cost")]
    [Export] public double BaseCost = 1;
    [Export] public double CostScalingPerLevel = 1.4f;
    

    public decimal PointsPerSecond => (decimal)(PointsPerTick * TickRate);

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

    public PointGeneratorState GetDefaultState()
    {
        return new()
        {
            GeneratorId = Descriptor.Id,
            Count = 0,
            Phase = 0,
            PointsPerTick = (decimal)PointsPerTick,
            SingleTickRate = (decimal)TickRate
        };
    }

    public decimal GetPointCostForLevel(int level)
    {
        return Math.Floor(GetPrecisePointCostForLevel(level));
    }

    public decimal GetPrecisePointCostForLevel(int level)
    {
        return (decimal)BaseCost * (decimal)Mathf.Pow(CostScalingPerLevel, level);
    }

    public decimal GetPointCostForLevels(int fromLevel, int toLevel) // inclusive, exclusive
    {
        return GetCumulativePointCostToLevel(toLevel - 1) - GetCumulativePointCostToLevel(fromLevel-1);
    }

    public decimal GetCumulativePointCostToLevel(int level)
    {
        return Math.Floor((decimal)BaseCost * ((decimal)(Mathf.Pow(CostScalingPerLevel, level+1) - 1)) / (decimal)(CostScalingPerLevel - 1));
    }

    public void TestMultiLevelCostFunction(int fromLevel, int toLevel) // inclusive, exclusive
    {
        // var bruteForceSolution = 0M;
        // for (var i = fromLevel; i < toLevel; i++) bruteForceSolution += GetPrecisePointCostForLevel(i);
        // bruteForceSolution = Math.Round(bruteForceSolution);
        //
        // decimal fastSolution = GetPointCostForLevels(fromLevel, toLevel);
        // if (Math.Abs(bruteForceSolution - fastSolution) > 1)
        // {
        //     GD.PrintErr($"Multi level cost function failed for inputs {fromLevel}, {toLevel}. Expected: {bruteForceSolution}; Actual: {fastSolution}");
        // }
    }
}
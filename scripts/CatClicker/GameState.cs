using System;
using Godot;

namespace CatClicker;

public struct GameState
{
    public double UnixTimestampSec;
    public decimal Points;

    public PointGeneratorState[] GeneratorStates;

    public GameState Advance()
    {
        double now = Time.GetUnixTimeFromSystem();
        double delta = now - UnixTimestampSec;

        decimal generatedPoints = 0;
        generatedPoints += AdvanceGenerators(GeneratorStates, delta, out var newGeneratorStates);

        return this with
        {
            UnixTimestampSec = now,
            Points = Points + generatedPoints,
            GeneratorStates = newGeneratorStates
        };
    }

    private static decimal AdvanceGenerators(PointGeneratorState[] generatorStates, double delta, out PointGeneratorState[] newGeneratorStates)
    {
        if (generatorStates == null)
        {
            newGeneratorStates = null;
            return 0;
        }
        if (delta == 0)
        {
            newGeneratorStates = CopyArray(generatorStates);
            return 0;
        }

        var generatedPoints = 0M;
        newGeneratorStates = new PointGeneratorState[generatorStates.Length];
        
        for (var i = 0; i < generatorStates.Length; i++)
        {
            generatedPoints += AdvanceGenerator(generatorStates[i], delta, out newGeneratorStates[i]);
        }

        return generatedPoints;
    }

    private static decimal AdvanceGenerator(PointGeneratorState generatorState, double delta, out PointGeneratorState newGeneratorState)
    {
        if (generatorState.TotalTickRate == 0)
        {
            newGeneratorState = generatorState;
            return 0;
        }
        decimal generationInterval = 1 / generatorState.TotalTickRate;

        decimal totalTicks = (decimal)(delta + generatorState.Phase) / generationInterval;
        var newPhase = (double)((totalTicks % 1) * generationInterval);
        totalTicks = Math.Floor(totalTicks);

        newGeneratorState = generatorState with
        {
            Phase = newPhase
        };
        return totalTicks * generatorState.PointsPerTick;
    }

    private static T[] CopyArray<T>(T[] arr)
    {
        if (arr == null) return null;
        var copy = new T[arr.Length];
        Array.Copy(arr, copy, arr.Length);
        return copy;
    }
}
using System;
using System.Numerics;
using Godot;

namespace CatClicker;

public struct GameState
{
    public double UnixTimestampSec;
    public BigInteger Points;

    public PointGeneratorState[] GeneratorStates;

    public GameState AdvanceTo(double now)
    {
        double delta = now - UnixTimestampSec;

        BigInteger generatedPoints = 0;
        generatedPoints += AdvanceGenerators(GeneratorStates, delta, out var newGeneratorStates);

        return this with
        {
            UnixTimestampSec = now,
            Points = Points + generatedPoints,
            GeneratorStates = newGeneratorStates
        };
    }

    private static BigInteger AdvanceGenerators(PointGeneratorState[] generatorStates, double delta, out PointGeneratorState[] newGeneratorStates)
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

        var generatedPoints = BigInteger.Zero;
        newGeneratorStates = new PointGeneratorState[generatorStates.Length];
        
        for (var i = 0; i < generatorStates.Length; i++)
        {
            generatedPoints += AdvanceGenerator(generatorStates[i], delta, out newGeneratorStates[i]);
        }

        return generatedPoints;
    }

    private static BigInteger AdvanceGenerator(PointGeneratorState generatorState, double delta, out PointGeneratorState newGeneratorState)
    {
        if (generatorState.TotalTickRate == 0)
        {
            newGeneratorState = generatorState;
            return 0;
        }
        decimal generationInterval = 1 / generatorState.TotalTickRate;

        decimal totalTicksFractional = (decimal)(delta + generatorState.Phase) / generationInterval;
        var newPhase = (double)((totalTicksFractional % 1) * generationInterval);
        BigInteger totalTicksInt = new(Math.Floor(totalTicksFractional));

        newGeneratorState = generatorState with
        {
            Phase = newPhase
        };
        return totalTicksInt * generatorState.PointsPerTick;
    }

    private static T[] CopyArray<T>(T[] arr)
    {
        if (arr == null) return null;
        var copy = new T[arr.Length];
        Array.Copy(arr, copy, arr.Length);
        return copy;
    }
}
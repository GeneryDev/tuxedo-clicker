using System;
using System.Collections.Generic;
using System.Numerics;
using GDF.Serialization;
using Godot;

namespace CatClicker;

public partial struct GameState : IJsonSerializable
{
    public double UnixTimestampSec;
    public BigInteger Points;

    public PointGeneratorState[] GeneratorStates;
    public ActiveEffectState[] ActiveEffectStates;

    public GameState AdvanceTo(double now)
    {
        double delta = now - UnixTimestampSec;

        return AdvanceBy(delta) with
        {
            UnixTimestampSec = now
        };
    }

    public GameState AdvanceBy(double delta)
    {
        var newState = this;
        while (delta > 0 && newState.GetRequiredIntermediateStep(delta) is var stepDelta)
        {
            newState = newState.AdvanceOnce(stepDelta);
            delta -= stepDelta;
        }

        return newState;
    }

    private GameState AdvanceOnce(double delta)
    {
        BigInteger generatedPoints = 0;
        generatedPoints += AdvanceGenerators(GeneratorStates, delta, out var newGeneratorStates, this);

        AdvanceEffects(ActiveEffectStates, delta, out var newEffectStates);

        return this with
        {
            UnixTimestampSec = UnixTimestampSec + delta,
            Points = Points + generatedPoints,
            GeneratorStates = newGeneratorStates,
            ActiveEffectStates = newEffectStates
        };
    }

    private double GetRequiredIntermediateStep(double maxDelta)
    {
        if (ActiveEffectStates != null)
        {
            foreach (var effect in ActiveEffectStates)
            {
                if (effect.RemainingSec < maxDelta) return effect.RemainingSec;
            }
        }
        return maxDelta;
    }

    private static BigInteger AdvanceGenerators<TModifier>(PointGeneratorState[] generatorStates, double delta, out PointGeneratorState[] newGeneratorStates, TModifier modifier) where TModifier : IProductionModifier
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
            generatedPoints += AdvanceGenerator(generatorStates[i], delta, out newGeneratorStates[i], modifier);
        }

        return generatedPoints;
    }

    private static BigInteger AdvanceGenerator<TModifier>(PointGeneratorState generatorState, double delta, out PointGeneratorState newGeneratorState, TModifier modifier) where TModifier : IProductionModifier
    {
        var generatorDef = PointGenerators.FromId(generatorState.GeneratorId).Resource;
        if (generatorDef == null)
        {
            newGeneratorState = generatorState;
            return 0;
        }
        decimal tickRate = (decimal)generatorDef.TickRate * generatorState.Count;
        BigInteger pointsPerTick = generatorDef.PointsPerTick;

        modifier.ModifyGeneratorProductionRate(generatorState, ref tickRate);
        
        if (tickRate == 0)
        {
            newGeneratorState = generatorState;
            return 0;
        }
        decimal generationInterval = 1 / tickRate;

        decimal totalTicksFractional = (decimal)(delta + generatorState.Phase) / generationInterval;
        var newPhase = (double)((totalTicksFractional % 1) * generationInterval);
        BigInteger totalTicksInt = new(Math.Floor(totalTicksFractional));

        newGeneratorState = generatorState with
        {
            Phase = newPhase
        };
        return totalTicksInt * pointsPerTick;
    }

    private static readonly List<ActiveEffectState> TempEffectStates = new();

    private static void AdvanceEffects(ActiveEffectState[] effectStates, double delta, out ActiveEffectState[] newEffectStates)
    {
        if (effectStates == null)
        {
            newEffectStates = null;
            return;
        }

        newEffectStates = new ActiveEffectState[effectStates.Length];
        TempEffectStates.Clear();
        
        foreach (var state in effectStates)
        {
            if (state.RemainingSec <= delta)
            {
                // expires
            }
            else
            {
                TempEffectStates.Add(state with {RemainingSec = state.RemainingSec - delta});
            }
        }

        newEffectStates = TempEffectStates.ToArray();
    }

    private static T[] CopyArray<T>(T[] arr)
    {
        if (arr == null) return null;
        var copy = new T[arr.Length];
        Array.Copy(arr, copy, arr.Length);
        return copy;
    }

    public bool HasEffect(StringName id)
    {
        if (ActiveEffectStates == null) return false;
        foreach (var effect in ActiveEffectStates)
        {
            if (effect.EffectId == id) return true;
        }

        return false;
    }

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(UnixTimestampSec), ref UnixTimestampSec);
        json.Deserialize(dict, nameof(Points), ref Points);
        json.Deserialize(dict, nameof(GeneratorStates), ref GeneratorStates);
        json.Deserialize(dict, nameof(ActiveEffectStates), ref ActiveEffectStates);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(UnixTimestampSec), ref UnixTimestampSec);
        json.Serialize(dict, nameof(Points), ref Points);
        json.Serialize(dict, nameof(GeneratorStates), ref GeneratorStates);
        json.Serialize(dict, nameof(ActiveEffectStates), ref ActiveEffectStates);
        return dict;
    }
}
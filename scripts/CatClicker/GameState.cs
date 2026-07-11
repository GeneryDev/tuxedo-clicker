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
    public BigInteger TotalGeneratedPoints;
    public BigInteger TotalClickedPoints;
    public long TotalClicks;
    public long BonusItemClicks;

    public PointGeneratorState[] GeneratorStates;
    public ActiveEffectState[] ActiveEffectStates;

    public GameProgressionData ProgressionData;

    public void AddClickedPoints(BigInteger points)
    {
        Points += points;
        TotalClickedPoints += points;
        TotalClicks++;
    }

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
        generatedPoints += AdvanceGenerators(GeneratorStates, delta, out var newGeneratorStates);

        AdvanceEffects(ActiveEffectStates, delta, out var newEffectStates);

        return this with
        {
            UnixTimestampSec = UnixTimestampSec + delta,
            Points = Points + generatedPoints,
            TotalGeneratedPoints = TotalGeneratedPoints + generatedPoints,
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

    private BigInteger AdvanceGenerators(PointGeneratorState[] generatorStates, double delta, out PointGeneratorState[] newGeneratorStates)
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

    private BigInteger AdvanceGenerator(PointGeneratorState generatorState, double delta, out PointGeneratorState newGeneratorState)
    {
        ComputeGeneratorProductionRate(generatorState, out decimal tickRate, out var pointsPerTick);
        
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

    public PointGeneratorState GetGeneratorState(StringName generatorId)
    {
        if (GeneratorStates != null)
        {
            foreach (var state in GeneratorStates)
            {
                if (state.GeneratorId == generatorId) return state;
            }
        }

        return default;
    }

    public PlayerFacingNumber ComputeTotalProductionRate(out decimal totalRate)
    {
        totalRate = 0;
        if (GeneratorStates != null)
        {
            foreach (var state in GeneratorStates)
            {
                ComputeGeneratorProductionRate(state, out decimal tickRate, out var pointsPerTick);
                totalRate += tickRate * pointsPerTick;
            }
        }

        return new PlayerFacingNumber(totalRate);
    }

    public PlayerFacingNumber ComputeGeneratorProductionRate(StringName generatorId, out decimal tickRate, out long pointsPerTick)
    {
        return ComputeGeneratorProductionRate(GetGeneratorState(generatorId), out tickRate, out pointsPerTick);
    }

    public PlayerFacingNumber ComputeGeneratorProductionRate(PointGeneratorState generatorState, out decimal tickRate, out long pointsPerTick)
    {
        var generatorDef = PointGenerators.FromId(generatorState.GeneratorId).Resource;
        if (generatorDef == null)
        {
            tickRate = 0;
            pointsPerTick = 0;
            return default;
        }
        tickRate = (decimal)generatorDef.TickRate * generatorState.Count;
        pointsPerTick = generatorDef.PointsPerTick;

        this.ModifyGeneratorProductionRate(generatorState, ref tickRate);
        return new PlayerFacingNumber(tickRate * pointsPerTick);
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
        json.Deserialize(dict, nameof(TotalGeneratedPoints), ref TotalGeneratedPoints);
        json.Deserialize(dict, nameof(TotalClickedPoints), ref TotalClickedPoints);
        json.Deserialize(dict, nameof(TotalClicks), ref TotalClicks);
        json.Deserialize(dict, nameof(BonusItemClicks), ref BonusItemClicks);
        json.Deserialize(dict, nameof(GeneratorStates), ref GeneratorStates);
        json.Deserialize(dict, nameof(ActiveEffectStates), ref ActiveEffectStates);
        json.Deserialize(dict, nameof(ProgressionData), ref ProgressionData);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(UnixTimestampSec), ref UnixTimestampSec);
        json.Serialize(dict, nameof(Points), ref Points);
        json.Serialize(dict, nameof(TotalGeneratedPoints), ref TotalGeneratedPoints);
        json.Serialize(dict, nameof(TotalClickedPoints), ref TotalClickedPoints);
        json.Serialize(dict, nameof(TotalClicks), ref TotalClicks);
        json.Serialize(dict, nameof(BonusItemClicks), ref BonusItemClicks);
        json.Serialize(dict, nameof(GeneratorStates), ref GeneratorStates);
        json.Serialize(dict, nameof(ActiveEffectStates), ref ActiveEffectStates);
        json.Serialize(dict, nameof(ProgressionData), ref ProgressionData);
        return dict;
    }
}
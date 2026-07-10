using System;
using System.Collections.Generic;
using GDF.Serialization;
using Godot;

namespace CatClicker;

public class GameProgressionData : IJsonSerializable
{
    public Dictionary<StringName, int> MaxOwnedGeneratorCounts;

    public void UpdateFromGameState(GameState state)
    {
        if (state.GeneratorStates != null)
        {
            foreach (var generatorState in state.GeneratorStates)
            {
                if (generatorState.Count > 0)
                {
                    int existingMax = GetMaxOwnedGeneratorCount(generatorState.GeneratorId);
                    MaxOwnedGeneratorCounts ??= new();
                    MaxOwnedGeneratorCounts[generatorState.GeneratorId] = Math.Max(existingMax, generatorState.Count);
                }
            }
        }
    }

    public int GetMaxOwnedGeneratorCount(StringName generatorId)
    {
        return MaxOwnedGeneratorCounts?.GetValueOrDefault(generatorId) ?? 0;
    }

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.DeserializeVariants(dict, nameof(MaxOwnedGeneratorCounts), ref MaxOwnedGeneratorCounts);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.SerializeVariants(dict, nameof(MaxOwnedGeneratorCounts), ref MaxOwnedGeneratorCounts);
        return dict;
    }
}
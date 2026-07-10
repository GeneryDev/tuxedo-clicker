using System;
using System.Collections.Generic;
using GDF.Serialization;
using Godot;

namespace CatClicker;

public class GameProgressionData : IJsonSerializable
{
    public Dictionary<StringName, int> MaxOwnedGeneratorCounts;
    public HashSet<StringName> ActiveUpgrades;

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

    public bool HasUpgrade(StringName upgradeId)
    {
        return ActiveUpgrades?.Contains(upgradeId) ?? false;
    }

    public void AddUpgrade(StringName upgradeId)
    {
        ActiveUpgrades ??= new();
        ActiveUpgrades.Add(upgradeId);
    }

    public void RemoveUpgrade(StringName upgradeId)
    {
        ActiveUpgrades ??= new();
        ActiveUpgrades.Remove(upgradeId);
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
        json.DeserializeVariants(dict, nameof(ActiveUpgrades), ref ActiveUpgrades);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.SerializeVariants(dict, nameof(MaxOwnedGeneratorCounts), ref MaxOwnedGeneratorCounts);
        json.SerializeVariants(dict, nameof(ActiveUpgrades), ref ActiveUpgrades);
        return dict;
    }
}
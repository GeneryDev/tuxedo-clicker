using System.Numerics;
using GDF.Serialization;
using Godot;

namespace CatClicker;

public struct PointGeneratorState : IJsonSerializable
{
    public StringName GeneratorId;
    public int Count;
    public double Phase;
    
    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(GeneratorId), ref GeneratorId);
        json.Deserialize(dict, nameof(Count), ref Count);
        json.Deserialize(dict, nameof(Phase), ref Phase);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(GeneratorId), ref GeneratorId);
        json.Serialize(dict, nameof(Count), ref Count);
        json.Serialize(dict, nameof(Phase), ref Phase);
        
        return dict;
    }
}
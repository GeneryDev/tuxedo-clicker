using GDF.Serialization;
using Godot;

namespace CatClicker;

public struct ActiveEffectState : IJsonSerializable
{
    public StringName EffectId;
    public double RemainingSec;
    
    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(EffectId), ref EffectId);
        json.Deserialize(dict, nameof(RemainingSec), ref RemainingSec);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(EffectId), ref EffectId);
        json.Serialize(dict, nameof(RemainingSec), ref RemainingSec);
        
        return dict;
    }
}
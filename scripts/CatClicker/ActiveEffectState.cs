using GDF.Serialization;
using Godot;

namespace CatClicker;

public struct ActiveEffectState : IJsonSerializable
{
    public StringName EffectId;
    public double RemainingSec;
    public double ApplicationDuration;

    public ActiveEffectState(StringName effectId, double duration)
    {
        EffectId = effectId;
        RemainingSec = ApplicationDuration = duration;
    }

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(EffectId), ref EffectId);
        json.Deserialize(dict, nameof(RemainingSec), ref RemainingSec);
        json.Deserialize(dict, nameof(ApplicationDuration), ref ApplicationDuration);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(EffectId), ref EffectId);
        json.Serialize(dict, nameof(RemainingSec), ref RemainingSec);
        json.Serialize(dict, nameof(ApplicationDuration), ref ApplicationDuration);
        
        return dict;
    }
}
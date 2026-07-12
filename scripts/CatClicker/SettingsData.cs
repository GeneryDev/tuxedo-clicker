using GDF.Serialization;
using Godot;

namespace CatClicker;

public class SettingsData : IJsonSerializable
{
    public float MasterVolume = 1.0f;
    public float SfxVolume = 1.0f;
    public float MusicVolume = 1.0f;
    public bool AutosaveEnabled = true;

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Deserialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Deserialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Deserialize(dict, nameof(AutosaveEnabled), ref AutosaveEnabled);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Serialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Serialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Serialize(dict, nameof(AutosaveEnabled), ref AutosaveEnabled);
        return dict;
    }
}
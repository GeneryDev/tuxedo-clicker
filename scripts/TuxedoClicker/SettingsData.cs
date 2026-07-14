using GDF.Serialization;
using GDF.UI;
using Godot;

namespace TuxedoClicker;

public class SettingsData : IJsonSerializable
{
    public float MasterVolume = 1.0f;
    public float SfxVolume = 1.0f;
    public float MusicVolume = 1.0f;
    public bool AutosaveEnabled = true;

    public GdfViewportUserSettings ViewportSettings = new();

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default with
        {
            PropertyOmissionHandlingMode = JsonSerializer.PropertyOmissionHandlingModeEnum.KeepDefault
        };
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Deserialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Deserialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Deserialize(dict, nameof(AutosaveEnabled), ref AutosaveEnabled);
        json.Deserialize(dict, nameof(ViewportSettings), ref ViewportSettings);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Serialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Serialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Serialize(dict, nameof(AutosaveEnabled), ref AutosaveEnabled);
        json.Serialize(dict, nameof(ViewportSettings), ref ViewportSettings);
        return dict;
    }
}
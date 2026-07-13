using GDF.Serialization;
using Godot;

namespace TuxedoClicker;

public class SettingsData : IJsonSerializable
{
    public float MasterVolume = 1.0f;
    public float SfxVolume = 1.0f;
    public float MusicVolume = 1.0f;
    public bool AutosaveEnabled = true;
    
    public DisplayServer.WindowMode FullscreenMode;
    public DisplayServer.WindowMode LastUsedFullscreenMode;
    public DisplayServer.WindowMode LastUsedWindowedMode;
    public Vector2I Resolution = default;

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default;
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Deserialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Deserialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Deserialize(dict, nameof(AutosaveEnabled), ref AutosaveEnabled);
        json.DeserializeEnum(dict, nameof(FullscreenMode), ref FullscreenMode);
        json.DeserializeEnum(dict, nameof(LastUsedFullscreenMode), ref LastUsedFullscreenMode);
        json.DeserializeEnum(dict, nameof(LastUsedWindowedMode), ref LastUsedWindowedMode);
        json.Deserialize(dict, nameof(Resolution), ref Resolution);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Serialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Serialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Serialize(dict, nameof(AutosaveEnabled), ref AutosaveEnabled);
        json.SerializeEnum(dict, nameof(FullscreenMode), ref FullscreenMode);
        json.SerializeEnum(dict, nameof(LastUsedFullscreenMode), ref LastUsedFullscreenMode);
        json.SerializeEnum(dict, nameof(LastUsedWindowedMode), ref LastUsedWindowedMode);
        json.Serialize(dict, nameof(Resolution), ref Resolution);
        return dict;
    }
}
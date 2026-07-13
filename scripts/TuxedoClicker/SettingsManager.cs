using GDF.Util;
using Godot;

namespace TuxedoClicker;

public partial class SettingsManager : SingletonNode<SettingsManager>
{
    [Signal]
    public delegate void UpdatedEventHandler();
    [Signal]
    public delegate void PropertyUpdatedEventHandler(string propertyName, Variant newValue);
    
    public SettingsData Settings = new();

    private bool _needsSaving = false;
    
    public void EmitChanged(string propertyName, Variant value)
    {
        _needsSaving = true;
        EmitSignalUpdated();
        EmitSignalPropertyUpdated(propertyName, value);
    }

    public override void _Ready()
    {
        base._Ready();
        this.Updated += OnUpdated;
        Load();
        OnUpdated();
    }

    private void OnUpdated()
    {
        AudioServer.SetBusVolumeLinear(0, Settings.MasterVolume);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("User SFX"), Settings.SfxVolume);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("User Music"), Settings.MusicVolume);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_needsSaving)
        {
            Save();
        }
    }
}
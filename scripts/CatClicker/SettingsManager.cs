using GDF.Util;
using Godot;

namespace CatClicker;

public partial class SettingsManager : SingletonNode<SettingsManager>
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    public SettingsData Settings = new();

    private bool _needsSaving = false;
    
    public void EmitChanged()
    {
        _needsSaving = true;
        EmitSignalUpdated();
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
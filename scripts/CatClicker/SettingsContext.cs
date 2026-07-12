using GDF.Data;
using GDF.Data.Static;
using GDF.Util;
using Godot;

namespace CatClicker;

[StaticDataContext("settings")]
public struct SettingsContext : IDataContext
{
    public SettingsData Settings => SettingsManager.Instance.Settings;
    
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!SettingsManager.InstanceExists) return false;
        switch (key)
        {
            case "master_volume":
            {
                output = Settings.MasterVolume;
                return true;
            }
            case "sfx_volume":
            {
                output = Settings.SfxVolume;
                return true;
            }
            case "music_volume":
            {
                output = Settings.MusicVolume;
                return true;
            }
            case "autosave_enabled":
            {
                output = Settings.AutosaveEnabled;
                return true;
            }
        }

        return false;
    }

    public bool WriteBack(string key, Variant value)
    {
        switch (key)
        {
            case "master_volume":
            {
                Settings.MasterVolume = value.AsSingle();
                SettingsManager.Instance.EmitChanged();
                return true;
            }
            case "sfx_volume":
            {
                Settings.SfxVolume = value.AsSingle();
                SettingsManager.Instance.EmitChanged();
                return true;
            }
            case "music_volume":
            {
                Settings.MusicVolume = value.AsSingle();
                SettingsManager.Instance.EmitChanged();
                return true;
            }
            case "autosave_enabled":
            {
                Settings.AutosaveEnabled = value.AsBool();
                SettingsManager.Instance.EmitChanged();
                return true;
            }
        }

        return false;
    }
    
    public bool EqualsContext(IDataContext other)
    {
        return other is SettingsContext;
    }

    public void ConnectUpdateSignal(Callable callable)
    {
        if (!SettingsManager.InstanceExists) return;
        SettingsManager.Instance.TryConnect(SettingsManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!SettingsManager.InstanceExists) return;
        SettingsManager.Instance.TryDisconnect(SettingsManager.SignalName.Updated, callable);
    }
}
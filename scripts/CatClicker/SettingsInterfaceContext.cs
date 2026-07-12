using System.Collections.Generic;
using GDF.Data;
using GDF.Data.Static;

namespace CatClicker;

[StaticDataContext("settings_interface")]
public struct SettingsInterfaceContext : IDataContext
{
    public bool GetContextCollection(string key, string input, List<IDataContext> output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "settings_fields":
            {
                output.Add(new SettingsFieldContext("label_general")
                {
                    Type = "header",
                    Name = "General"
                }.Boxed());
                output.Add(new SettingsFieldContext("autosave_enabled")
                {
                    Type = "checkbox",
                    Name = "Autosave (60s)"
                }.Boxed());
                
                
                output.Add(new SettingsFieldContext("label_audio")
                {
                    Type = "header",
                    Name = "Audio"
                }.Boxed());
                output.Add(new SettingsFieldContext("master_volume")
                {
                    Type = "volume_slider",
                    Name = "Master Volume"
                }.Boxed());
                output.Add(new SettingsFieldContext("sfx_volume")
                {
                    Type = "volume_slider",
                    Name = "SFX Volume"
                }.Boxed());
                output.Add(new SettingsFieldContext("music_volume")
                {
                    Type = "volume_slider",
                    Name = "Music Volume"
                }.Boxed());
                
                return true;
            }
        }

        return false;
    }

    public bool EqualsContext(IDataContext other)
    {
        return other is SettingsInterfaceContext;
    }
}
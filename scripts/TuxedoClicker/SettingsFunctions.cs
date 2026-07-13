using Godot;

namespace TuxedoClicker;

public partial class SettingsFunctions : Node
{
    public void SetSetting(string key, Variant value)
    {
        new SettingsContext().WriteBack(key, value);
    }
}
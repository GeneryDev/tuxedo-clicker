using Godot;

namespace CatClicker;

public partial class SettingsManager
{
    public static readonly string SettingsFilePath = "user://settings.json";
    
    public void Save()
    {
        DirAccess.MakeDirRecursiveAbsolute(SettingsFilePath.GetBaseDir());
        string serialized = Json.Stringify(Instance.Settings.Serialize(), "\t", false, fullPrecision: true);
        using (var fa = FileAccess.Open(SettingsFilePath, FileAccess.ModeFlags.Write))
        {
            fa.StoreString(serialized);
        }
        _needsSaving = false;
        EmitSignalUpdated();
    }

    public bool Load()
    {
        if (!FileAccess.FileExists(SettingsFilePath))
        {
            GD.Print("No settings file exists");
            return false;
        }
        using (var fa = FileAccess.Open(SettingsFilePath, FileAccess.ModeFlags.Read))
        {
            string serialized = fa.GetAsText(true);
            var parsed = Json.ParseString(serialized);
            if (parsed.VariantType != Variant.Type.Nil)
            {
                var newData = new SettingsData();
                newData.Deserialize(parsed);
                Settings = newData;
                EmitSignalUpdated();
                return true;
            }
        }

        return false;
    }
}
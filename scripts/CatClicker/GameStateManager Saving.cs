using System.Collections.Generic;
using Godot;

namespace CatClicker;

public partial class GameStateManager
{
    public static readonly string SaveFilePath = "user://save.json";

    private bool _saveDirty = false;
    
    public void Autosave()
    {
        Save();
    }

    private void MarkDirty()
    {
        _saveDirty = true;
    }

    public bool IsDirty() => _saveDirty;
    
    public void Save()
    {
        Step();
        
        DirAccess.MakeDirRecursiveAbsolute(SaveFilePath.GetBaseDir());
        string serialized = Json.Stringify(Instance.State.Serialize(), "\t", false, fullPrecision: true);
        using (var fa = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write))
        {
            fa.StoreString(serialized);
        }
        GD.Print("Saved!");
        _saveDirty = false;
        EmitSignalUpdated();
    }

    public void Load()
    {
        if (!LoadFromDisk())
        {
            LoadState(NewBlankState());
        }
    }

    public bool LoadFromDisk()
    {
        if (!FileAccess.FileExists(SaveFilePath))
        {
            GD.Print("No save file exists");
            return false;
        }
        using (var fa = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Read))
        {
            string serialized = fa.GetAsText(true);
            var parsed = Json.ParseString(serialized);
            if (parsed.VariantType != Variant.Type.Nil)
            {
                var newState = new GameState();
                newState.Deserialize(parsed);
                return LoadState(newState);
            }
        }

        return false;
    }

    private bool LoadState(GameState newState)
    {
        SanitizeState(ref newState);
        State = newState;
        GD.Print("Loaded!");
        var pointsGeneratedSinceSave = GetCurrentState().Points - State.Points;
        double timeElapsedSinceSave = GetCurrentState().UnixTimestampSec - State.UnixTimestampSec;
        _saveDirty = false;
        if (timeElapsedSinceSave > 0)
        {
            MessageChannel.BroadcastMessage("effect_msg", $"Generated {GameInterfaceManager.Instance.FormatNumber(pointsGeneratedSinceSave)} points over {GameInterfaceManager.Instance.FormatTime(timeElapsedSinceSave)}");
        }
        EmitSignalUpdated();
        return true;
    }

    private static void SanitizeState(ref GameState state)
    {
        // Sanitize generator states (add any missing generators and normalize order in list)
        var sanitizedGeneratorStates = new List<PointGeneratorState>();
        foreach (var generator in PointGenerators.CollectAll(new()))
        {
            PointGeneratorState sanitizedState = new() {GeneratorId = generator.Id};
            if (state.GeneratorStates != null)
            {
                foreach (var incomingState in state.GeneratorStates)
                {
                    if (incomingState.GeneratorId == generator.Id)
                    {
                        sanitizedState = incomingState;
                    }
                }
            }
            
            sanitizedGeneratorStates.Add(sanitizedState);
        }

        state.GeneratorStates = sanitizedGeneratorStates.ToArray();
        
        // Sanitize effect states (add list if missing)
        state.ActiveEffectStates ??= System.Array.Empty<ActiveEffectState>();
        
        // Sanitize progression data (add if missing)
        state.ProgressionData ??= new();
    }

    public static GameState NewBlankState()
    {
        var state = new GameState()
        {
            UnixTimestampSec = Instance.Now
        };
        SanitizeState(ref state);
        return state;
    }
}
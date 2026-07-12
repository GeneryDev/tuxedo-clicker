using GDF.Debug;
using Godot;

namespace CatClicker;

[HasDebugCommands]
public partial class GameStateManager
{
    [DebugCommand("ffw")]
    public static void FastForward()
    {
        var minutes = 5;
        Instance.State.UnixTimestampSec -= (double)minutes * 60;
    }
    [DebugCommand("serialize")]
    public static void Serialize()
    {
        var serialized = Instance.State.Serialize();
        GD.Print("========");
        GD.Print($"Timestamp actually: {Instance.State.UnixTimestampSec}");
        GD.Print($"Points actually: {Instance.State.Points}");
        var stringified = Json.Stringify(serialized, "\t", sortKeys: false, fullPrecision: true);
        GD.Print(stringified);
        var reparsed = new GameState();
        reparsed.Deserialize(Json.ParseString(stringified));
        string roundTrip = Json.Stringify(reparsed.Serialize(), "\t", sortKeys: false, fullPrecision: true);
        GD.Print($"Round-trip: {roundTrip}");
        GD.Print("========");
    }
    [DebugCommand("save")]
    public static void DebugSave()
    {
        Instance.Save();
    }
    [DebugCommand("load")]
    public static void DebugLoad()
    {
        Instance.Load();
    }
    [DebugCommand("reset")]
    public static void Reset()
    {
        Instance.LoadState(NewBlankState());
    }
    [DebugCommand("ending")]
    public static void DebugEnding()
    {
        Instance.EmitSignalEndingReached();
    }
}
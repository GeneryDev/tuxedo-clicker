using GDF.Debug;
using Godot;

namespace TuxedoClicker;

[HasDebugCommands]
public partial class GameStateManager
{
    [DebugCommand("ffw", DebugCommandType.TriggerWithArguments)]
    public static void FastForward(DebugCommandArgumentParser args)
    {
        int count = 0;
        double unit = 0;
        if (args.ReachedEnd())
        {
            count = 5;
            unit = 60;
        }
        else if (args.ReadInt(out count))
        {
            // ok
            if (!args.ReachedEnd())
            {
                if (args.ReadString(out var rawUnit))
                {
                    switch (rawUnit)
                    {
                        case "s":
                        case "sec":
                        case "secs":
                        case "second":
                        case "seconds":
                            unit = 1;
                            break;
                        case "m":
                        case "min":
                        case "mins":
                        case "minute":
                        case "minutes":
                            unit = 60;
                            break;
                        case "h":
                        case "hr":
                        case "hrs":
                        case "hour":
                        case "hours":
                            unit = 60*60;
                            break;
                        case "d":
                        case "day":
                        case "days":
                            unit = 60*60*24;
                            break;
                        case "mo":
                        case "mos":
                        case "month":
                        case "months":
                            unit = 60*60*24*30;
                            break;
                        case "y":
                        case "yr":
                        case "yrs":
                        case "year":
                        case "years":
                            unit = 60*60*24*365;
                            break;
                        default:
                        {
                            args.PrintCustomError($"Unknown time unit '{rawUnit}'");
                            return;
                        }
                    }
                }
                else
                {
                    args.PrintError();
                }
            }
        }
        else
        {
            args.PrintError();
            return;
        }
        MessageChannel.BroadcastMessage("effect_msg", $"Fast-forwarded {GameInterfaceManager.Instance.FormatTime(count * unit)}");
        Instance.State.UnixTimestampSec -= count * unit;
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
    public static void DebugReset()
    {
        Instance.LoadState(NewBlankState());
    }
    [DebugCommand("ending")]
    public static void DebugEnding()
    {
        Instance.EmitSignalEndingReached();
    }
}
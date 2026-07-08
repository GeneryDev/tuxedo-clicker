using GDF.Debug;

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
}
using GDF.Util;
using Godot;

namespace CatClicker;

public partial class GameStateManager : SingletonNode<GameStateManager>
{
    public GameState State = new();

    public override void _Ready()
    {
        State = new GameState()
        {
            UnixTimestampSec = Time.GetUnixTimeFromSystem(),
            Points = 0,
            GeneratorStates = new PointGeneratorState[]
            {
                new PointGeneratorState()
                {
                    GeneratorId = "hand",
                    Count = 1,
                    Phase = 0,
                    PointsPerTick = 1,
                    SingleTickRate = 1
                }
            }
        };
    }

    public void Update()
    {
        // test only

        State = State.Advance();
    }

    public GameState GetCurrentState()
    {
        return State.Advance();
    }

    public void Click()
    {
        State = State.Advance();
        State.Points++;
    }
}
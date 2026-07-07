using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

public partial class BonusSpawner : Node
{
    [Export]
    public NodeTemplate Template;
    [Export]
    public float BaseChancePerSecond = 0.01f;

    private RandomNumberGenerator _rng = new();

    private Accumulator _timer;

    public void Spawn()
    {
        var task = Template.New<Control>();
        var container = (Control)task.Container;
        task.Insert();

        task.Instance.Position = new Vector2(_rng.Randf(), _rng.Randf()) * container.Size;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _timer.Add((float)delta);
        while (_timer.Consume(1))
        {
            AttemptSpawn();
        }
    }

    private float GetSpawnChance()
    {
        return BaseChancePerSecond;
    }

    private void AttemptSpawn()
    {
        if (_rng.Randf() < GetSpawnChance())
        {
            Spawn();
        }
    }
}
using System;
using GDF.Data;
using GDF.Debug;
using GDF.Multiplayer;
using GDF.Util;
using Godot;

namespace CatClicker;

[HasDebugCommands]
public partial class BonusSpawner : Node
{
    public static readonly StringName Group = "bonus_spawner";
    
    [Export]
    public NodeTemplate Template;
    [Export]
    public float BaseChancePerSecond = 0.01f;

    private RandomNumberGenerator _rng = new();

    private Accumulator _timer;

    public override void _EnterTree()
    {
        base._EnterTree();
        this.AddToGroup(Group);
    }

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
        float chance = BaseChancePerSecond;
        if (GameStateManager.Instance.State.ProgressionData.HasUpgrade("box_tier_1"))
        {
            chance = 1 - Mathf.Pow(1 - chance, 2);
        }
        return chance;
    }

    private void AttemptSpawn()
    {
        if (_rng.Randf() < GetSpawnChance())
        {
            Spawn();
        }
    }

    [DebugCommand("bonus")]
    public static void DebugSpawn()
    {
        foreach (var node in GameStateManager.Instance.GetTree().GetNodesInGroup(Group))
        {
            if (node is not BonusSpawner spawner) continue;
            spawner.Spawn();
        }
    }
}
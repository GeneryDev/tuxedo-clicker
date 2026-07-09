using GDF.Data;
using Godot;

namespace CatClicker;

public partial class MessageChannel : Node
{
    [Export] public StringName Group;
    [Export]
    public NodeTemplate Template;

    [Export] public bool SpawnAtMousePosition = false;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        this.AddToGroup(Group);
    }

    public void ReceiveMessage(string text)
    {
        var task = Template.New<Control>();
        task.Instance.InjectContext(new MessageContext(text));
        task.Insert();
        if (SpawnAtMousePosition)
        {
            var window = GetWindow();
            task.Instance.GlobalPosition = window.GetFinalTransform().AffineInverse() * window.GetMousePosition();
        }
    }

    public static void BroadcastMessage(string group, string text)
    {
        foreach (var node in GameStateManager.Instance.GetTree().GetNodesInGroup(group))
        {
            if(node is MessageChannel ch && ch.Group == group) ch.ReceiveMessage(text);
        }
    }
}

public struct MessageContext : IDataContext
{
    public string Text;

    public MessageContext(string text)
    {
        Text = text;
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "text":
            {
                output = Text;
                return true;
            }
        }

        return false;
    }

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "text":
            {
                replacement = Text;
                return true;
            }
        }

        return false;
    }
}
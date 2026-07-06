using GDF.Data;
using Godot;

namespace CatClicker;

[Tool]
[GlobalClass]
public partial class PointGenerator : Resource, IDataContext
{
    [Export] public string DisplayName;

    public PointGenerators.Descriptor Descriptor => PointGenerators.From(this);

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "id":
            {
                output = Descriptor.Id;
                return true;
            }
            case "name":
            {
                output = DisplayName;
                return true;
            }
        }

        return false;
    }
}
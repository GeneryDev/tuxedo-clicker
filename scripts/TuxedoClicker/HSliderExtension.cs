using GDF.Data;
using Godot;

namespace TuxedoClicker;

public partial class HSliderExtension : HSlider, IDataContext
{
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "value":
            {
                output = Value;
                return true;
            }
        }

        return false;
    }
}
using System.Globalization;
using GDF.Serialization;
using System.Numerics;
using Godot;

namespace CatClicker;

public static class CustomJsonSerializers
{
    public static void Deserialize(this JsonSerializer json, Godot.Collections.Dictionary dict, string fieldName,
        ref BigInteger field)
    {
        string propertyName = JsonSerializer.ProtectedMethods.GetPropertyName(json, fieldName);
        if ((dict?.TryGetValue(propertyName, out var v) ?? false) && v.VariantType == Variant.Type.String)
        {
            string raw = v.AsString();
            if (!BigInteger.TryParse(raw, out field))
            {
                GD.PrintErr($"Failed to deserialize property '{propertyName}' big integer number '{raw}': invalid format");
            }
        }
        else if(json.PropertyOmissionHandlingMode == JsonSerializer.PropertyOmissionHandlingModeEnum.UseTypeDefault) field = default;
        
    }
    public static void Serialize(this JsonSerializer json, Godot.Collections.Dictionary dict, string fieldName,
        ref BigInteger field)
    {
        string propertyName = JsonSerializer.ProtectedMethods.GetPropertyName(json, fieldName);
        var serialized = field.ToString(NumberFormatInfo.InvariantInfo);
        dict[propertyName] = serialized;
    }
}
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using GDF.Serialization;
using Godot;

namespace TuxedoClicker;

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
    public static void DeserializeVariants<[MustBeVariant]T>(this JsonSerializer json, Godot.Collections.Dictionary dict, string fieldName, ref HashSet<T> field)
    {
        string propertyName = JsonSerializer.ProtectedMethods.GetPropertyName(json, fieldName);
        if (dict?.TryGetValue(propertyName, out var v) ?? false)
        {
            var list = field ?? new HashSet<T>();
            list.Clear();
            foreach (var rawValue in v.AsGodotArray())
            {
                list.Add(rawValue.As<T>());
            }
            field = list;
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
    public static void SerializeVariants<[MustBeVariant]T>(this JsonSerializer json, Godot.Collections.Dictionary dict, string fieldName, ref HashSet<T> field)
    {
        string propertyName = JsonSerializer.ProtectedMethods.GetPropertyName(json, fieldName);
        if (field is {Count: > 0})
        {
            var arr = new Godot.Collections.Array();
            foreach (var value in field)
            {
                arr.Add(Variant.From(value));
            }

            dict[propertyName] = arr;
        }
        else dict.Remove(propertyName);
    }
}
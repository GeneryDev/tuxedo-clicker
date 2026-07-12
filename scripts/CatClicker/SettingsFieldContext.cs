using GDF.Data;
using GDF.Util;
using Godot;

namespace CatClicker;

public struct SettingsFieldContext : IDataContext, ICacheableDataContext<SettingsFieldContext>
{
    public string Key;
    public string Type;
    public string Name;

    public SettingsFieldContext(string key)
    {
        Key = key;
        Name = key.Capitalize();
    }

    public Variant GetValue()
    {
        Variant value = default;
        if (new SettingsContext().GetContextVariable(Key, "", ref value, null))
        {
            return value;
        }

        return default;
    }

    public void SetValue(Variant value)
    {
        new SettingsContext().WriteBack(Key, value);
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!SettingsManager.InstanceExists) return false;
        switch (key)
        {
            case "key":
            {
                output = Key;
                return true;
            }
            case "type":
            {
                output = Type;
                return true;
            }
            case "value":
            {
                output = GetValue();
                return true;
            }
        }

        return false;
    }

    public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
    {
        switch (key)
        {
            case "name":
            {
                replacement = Name;
                return true;
            }
        }

        return false;
    }

    public bool EqualsContext(IDataContext other)
    {
        return other is SettingsFieldContext otherCtx && otherCtx.Key == Key;
    }

    public void ConnectUpdateSignal(Callable callable)
    {
        if (!SettingsManager.InstanceExists) return;
        SettingsManager.Instance.TryConnect(SettingsManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!SettingsManager.InstanceExists) return;
        SettingsManager.Instance.TryDisconnect(SettingsManager.SignalName.Updated, callable);
    }

    public bool EqualsContext(SettingsFieldContext otherCtx)
    {
        return otherCtx.Key == Key && otherCtx.Type == this.Type;
    }

    public bool CanCache()
    {
        return true;
    }
}
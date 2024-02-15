using System;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Types;

public class Variant : IEquatable<Variant>
{
    #region Properties

    private string? _stringValue;

    public string? StringValue
    {
        get { return _stringValue; }
        set { Nullify(); _stringValue = value; }
    }

    private List<Variant>? _listValue;

    public List<Variant>? ListValue
    {
        get { return _listValue; }
        set { Nullify(); _listValue = value; }
    }

    private Dictionary<string, Variant>? _dictValue;

    public Dictionary<string, Variant>? DictValue
    {
        get { return _dictValue; }
        set { Nullify(); _dictValue = value; }
    }

    #endregion Properties

    #region Constructors

    public Variant()
    {
    }

    public Variant(string value)
    {
        _stringValue = value;
    }

    public Variant(List<Variant> value)
    {
        _listValue = value;
    }

    public Variant(Dictionary<string, Variant> value)
    {
        _dictValue = value;
    }

    #endregion Constructors

    #region Is Types

    public bool IsString
    {
        get { return _stringValue is not null; }
    }

    public bool IsList
    {
        get { return _listValue is not null; }
    }

    public bool IsDict
    {
        get { return _dictValue is not null; }
    }

    #endregion Is Types

    #region Explicit Conversions Variant to string/udo/lst/dict

    public static explicit operator string(Variant d)
    {
        return d.ToString();
    }

    public static explicit operator List<Variant>?(Variant d)
    {
        return d.ListValue;
    }

    public static explicit operator List<string>?(Variant d)
    {
        return d.ListValue?
            .Select(v => v.StringValue)
            .Where(v => v is not null)
            .Cast<string>()
            .ToList() ?? null;
    }

    public static explicit operator Dictionary<string, Variant>?(Variant d)
    {
        return d.DictValue;
    }

    #endregion Explicit Conversions Variant to string/udo/lst/dict

    #region Implicit Conversions string/udo/lst/dict to Variant

    public static implicit operator Variant(string d)
    {
        return new Variant(d);
    }

    public static implicit operator Variant(List<Variant> d)
    {
        return new Variant(d);
    }

    public static implicit operator Variant(List<string> d)
    {
        return new Variant(d.Select(v => new Variant(v)).ToList());
    }

    public static implicit operator Variant(Dictionary<string, Variant> d)
    {
        return new Variant(d);
    }

    #endregion Implicit Conversions string/udo/lst/dict to Variant

    private void Nullify()
    {
        _stringValue = null;
        _listValue = null;
        _dictValue = null;
    }

    public override string ToString()
    {
        if (IsString)
        {
            return _stringValue ?? string.Empty;
        }

        if (IsList)
        {
            return "(List) Count: " + (_listValue?.Count ?? -1);
        }

        return IsDict
            ? "(Dict) Count: " + (_dictValue?.Count ?? -1)
            : string.Empty;
    }

    public bool Equals(Variant? val)
    {
        if (val is null)
            return false;

        if (IsString && val.IsString) return StringValue == val.StringValue;
        if (IsList && val.IsList)
        {
            if (ListValue is not null
                && val.ListValue is not null
                && ListValue.Count == val.ListValue.Count
            )
            {
                for (var i = 0; i < ListValue.Count; i++)
                {
                    if (!ListValue[i].Equals(val.ListValue[i])) return false;
                }
            }
        }
        if (IsDict && val.IsDict)
        {
            if (
                   DictValue is not null
                && val.DictValue is not null
                && DictValue.Count == val.DictValue.Count
            )
            {
                foreach (var kv in DictValue)
                {
                    if (!(val.DictValue.ContainsKey(kv.Key) &&
                          kv.Value.Equals(val.DictValue[kv.Key]))) return false;
                }
            }
        }

        // Theoretically impossible at the time of writing but if we do somehow end
        // up with empty variants later we'll consider them equal.
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Variant);
    }
}
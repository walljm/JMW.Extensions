using JMW.Extensions.Numbers;
using JMW.Parsing.Compile;
using System;
using System.Collections.Generic;

namespace JMW.Parsing.Extractors;

public abstract class Base
{
    public const string QUANTIFIER = "q";
    public const string INDEX = "i";

    public Base(Tag t)
    {
        if (t.Properties.TryGetValue(Search.SEARCH, out Tag searchTag))
        {
            foreach (var val in (Stack<Tag>)searchTag.Value)
            {
                this.Search.Query.Add(val.Value.ToString());
            }

            if (searchTag.Properties.TryGetValue(Search.MODS, out Tag modsTag))
            {
                this.Search.Mods = modsTag.Value.ToString();
            }
        }

        if (t.Properties.TryGetValue(QUANTIFIER, out Tag quantifierTag))
        {
            var n = quantifierTag.Value.ToString().ToIntOrNeg1();
            if (n == -1)
            {
                throw new ParseException(INDEX + " must be a 32 bit integer.");
            }
            this.Quantifier = n;
        }

        if (t.Properties.TryGetValue(INDEX, out Tag indexTag))
        {
            var n = indexTag.Value.ToString().ToIntOrNeg1();
            if (n == -1)
            {
                throw new ParseException(INDEX + " must be a 32 bit integer.");
            }
            this.Index = n;
        }
    }

    public Base(List<string> search, string mods, int q, int idx)
    {
        this.Search.Query = search;
        this.Search.Mods = mods;
        this.Quantifier = q;
        this.Index = idx;
    }

    public Search Search { get; set; } = new();
    public int Quantifier { get; set; } = -1;
    public int Index { get; set; } = -1;

    public abstract string Parse(string s);

    public static Base ToExtractor(Tag t)
    {
        return t.Name switch
        {
            To.NAME => new To(t),
            After.NAME => new After(t),
            Split.NAME => new Split(t),
            Column.NAME => new Column(t),
            _ => throw new ArgumentException("Unsupported Expression Tag", nameof(t)),
        };
    }
}
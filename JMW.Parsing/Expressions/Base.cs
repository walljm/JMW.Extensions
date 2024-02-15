﻿using JMW.Parsing.Compile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Parsing.Expressions;

public abstract class Base : IExpression
{
    public Base(List<string> search, string mods)
    {
        this.Search.Mods = mods;
        this.Search.Query = search;
    }

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
    }

    public Search Search { get; set; } = new Search();

    public abstract bool Test(string s);

    /// <summary>
    /// Uses a function to test against a provided string.
    /// </summary>
    /// <param name="s">String to search against</param>
    /// <param name="func">Function that takes the String to test against and a value to test for and returns a boolean.</param>
    public bool Test(string s, Func<string, string, bool> func)
    {
        bool v = this.Search.Mods.Contains(Constants.CASE_INSENSITIVE) // run case insensitive
            ? this.Search.Query.Any(sr => func(s.ToLower(), sr.ToLower()))
            : this.Search.Query.Any(sr => func(s, sr));

        return this.Search.Mods.Contains(Constants.NEGATE) ? !v : v;
    }

    public static IExpression ToExpression(Tag t)
    {
        return t.Name switch
        {
            Contains.NAME => new Contains(t),
            StartsWith.NAME1 or StartsWith.NAME2 => new StartsWith(t),
            EndsWith.NAME1 or EndsWith.NAME2 => new EndsWith(t),
            RegularExpression.NAME1 or RegularExpression.NAME2 => new RegularExpression(t),
            And.NAME => new And(t),
            Or.NAME => new Or(t),
            Count.NAME => new Count(t),
            Look.NAME => new Look(t),
            _ => throw new ArgumentException("Unsupported Expression Tag", nameof(t)),
        };
    }
}
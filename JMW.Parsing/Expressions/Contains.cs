using JMW.Parsing.Compile;
using System;
using System.Collections.Generic;

namespace JMW.Parsing.Expressions;

public class Contains : Base
{
    public const string NAME = "contains";

    public Contains(Tag t) : base(t)
    {
    }

    public Contains(List<string> search, string mods) : base(search, mods)
    {
    }

    public override bool Test(string s)
    {
        var compareOptions = this.Search.Mods.Contains(Constants.CASE_INSENSITIVE)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        return base.Test(s, (str, sch) => str.Contains(sch, compareOptions));
    }
}
﻿using System.Collections.Generic;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class EndsWith : Base
    {
        public const string NAME = "endswith";

        public EndsWith(Tag t) : base(t)
        {
        }

        public EndsWith(List<string> search, string mods) : base(search, mods)
        {
        }

        public override bool Test(string s)
        {
            return base.Test(s, (str, sch) => str.EndsWith(sch));
        }
    }
}
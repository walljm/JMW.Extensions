using System.Collections.Generic;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class StartsWith : Base
    {
        public const string NAME = "startswith";

        public StartsWith(Tag t) : base(t)
        {
        }

        public StartsWith(List<string> search, string mods) : base(search, mods)
        {
        }

        public override bool Test(string s)
        {
            return base.Test(s, (str, sch) => str.StartsWith(sch));
        }
    }
}
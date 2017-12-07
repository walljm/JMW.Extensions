using System.Collections.Generic;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
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
            return base.Test(s, (str, sch) => str.Contains(sch));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
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
            bool v;

            if (Mods.Contains("i"))
                v = Search.Any(sr => s.ToLower().StartsWith(sr.ToLower()));
            else
                v = Search.Any(s.StartsWith);

            return Mods.Contains("n") ? !v : v;
        }
    }
}
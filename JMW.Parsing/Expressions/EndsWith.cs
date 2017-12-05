using System.Linq;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class EndsWith : Base
    {
        public const string NAME = "endswith";

        public EndsWith(Tag t) : base(t)
        {
        }

        public override bool Test(string s)
        {
            bool v;

            if (Mods.Contains("i"))
                v = Search.Any(sr => s.ToLower().EndsWith(sr.ToLower()));
            else
                v = Search.Any(s.EndsWith);

            return Mods.Contains("n") ? !v : v;
        }
    }
}
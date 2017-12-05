using System.Linq;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class Contains : Base
    {
        public const string NAME = "contains";

        public Contains(Tag t) : base(t)
        {
        }

        public override bool Test(string s)
        {
            bool v;
            if (Mods.Contains("i"))
                v = Search.Any(sr => s.ToLower().Contains(sr.ToLower()));
            else
                v = Search.Any(s.Contains);

            return Mods.Contains("n") ? !v : v;
        }
    }
}
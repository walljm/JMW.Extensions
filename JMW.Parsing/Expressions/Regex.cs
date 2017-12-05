using System.Linq;
using System.Text.RegularExpressions;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class Regex : Base
    {
        public const string NAME = "regex";

        public Regex(Tag t) : base(t)
        {
        }

        public override bool Test(string s)
        {
            bool v;
            if (Mods.Contains("i"))
                v = Search.Any(sr => System.Text.RegularExpressions.Regex.IsMatch(s, sr, RegexOptions.IgnoreCase));
            else
                v = Search.Any(sr => System.Text.RegularExpressions.Regex.IsMatch(s, sr));

            return Mods.Contains("n") ? !v : v;
        }
    }
}
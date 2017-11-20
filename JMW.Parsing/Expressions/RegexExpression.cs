using System.Linq;
using System.Text.RegularExpressions;

namespace JMW.Parsing.Expressions
{
    public class RegexExpression : ExpressionBase
    {
        public const string NAME = "regex";

        public override bool Test(string s)
        {
            bool v;
            if (Mods.Contains("i"))
                v = Search.Any(sr => Regex.IsMatch(s, sr, RegexOptions.IgnoreCase));
            else
                v = Search.Any(sr => Regex.IsMatch(s, sr));

            return Mods.Contains("n") ? !v : v;
        }

        public RegexExpression(Tag t) : base(t)
        {
        }
    }
}
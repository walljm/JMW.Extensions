using System.Linq;

namespace JMW.Parsing.Expressions
{
    public class StartsWithExpression : ExpressionBase
    {
        public const string NAME = "startswith";

        public StartsWithExpression(Tag t) : base(t)
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
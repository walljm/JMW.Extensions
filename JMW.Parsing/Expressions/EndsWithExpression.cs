using System.Linq;

namespace JMW.Parsing.Expressions
{
    public class EndsWithExpression : ExpressionBase
    {
        public const string NAME = "endswith";

        public EndsWithExpression(Tag t) : base(t)
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
using System.Linq;

namespace JMW.Parsing.Expressions
{
    public class ContainsExpression : ExpressionBase
    {
        public const string NAME = "contains";

        public ContainsExpression(Tag t) : base(t)
        {
            
        }

        public override bool Test(string s)
        {
            bool v;
            if (Mods.Contains("i"))
                v = Search.Any(sr => s.ToLower().Contains(sr.ToLower()));
            else
                v = Search.Any( s.Contains);

            return Mods.Contains("n") ? !v : v;
        }
    }
}
using System.Collections.Generic;

namespace JMW.Parsing.Expressions
{
    public class OrExpression : IExpression
    {
        public const string NAME = "or";

        public Stack<IExpression> Expressions = new Stack<IExpression>();

        public OrExpression(Tag t)
        {
            if (t.Value is Stack<Tag> tags)
            {
                foreach (var tag in tags)
                    Expressions.Push(ExpressionBase.ToExpression(tag));
            }
        }

        public bool Test(string s)
        {
            foreach (var exp in Expressions)
            {
                if (exp.Test(s))
                    return true;
            }

            return false;
        }
    }
}
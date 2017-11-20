using System.Collections.Generic;

namespace JMW.Parsing.Expressions
{
    public class AndExpression : IExpression
    {
        public const string NAME = "and";

        public Stack<IExpression> Expressions = new Stack<IExpression>();

        public AndExpression(Tag t)
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
                if (!exp.Test(s))
                    return false;
            }

            return true;
        }
    }
}
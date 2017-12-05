using System.Collections.Generic;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class And : IExpression
    {
        public const string NAME = "and";

        public Stack<IExpression> Expressions = new Stack<IExpression>();

        public And(Tag t)
        {
            if (t.Value is Stack<Tag> tags)
            {
                foreach (var tag in tags)
                    Expressions.Push(Base.ToExpression(tag));
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
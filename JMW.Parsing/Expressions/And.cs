using JMW.Parsing.Compile;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Parsing.Expressions;

public class And : IExpression
{
    public const string NAME = "and";

    public Stack<IExpression> Expressions = new();

    public And(Tag t)
    {
        if (t.Value is Stack<Tag> tags)
        {
            foreach (var tag in tags)
            {
                this.Expressions.Push(Base.ToExpression(tag));
            }
        }
    }

    public And(IEnumerable<IExpression> expressions)
    {
        foreach (var exp in expressions)
        {
            this.Expressions.Push(exp);
        }
    }

    public bool Test(string s)
    {
        return !this.Expressions.Any(o => !o.Test(s));
    }
}
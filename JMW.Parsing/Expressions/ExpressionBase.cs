using System;
using System.Collections.Generic;

namespace JMW.Parsing.Expressions
{
    public abstract class ExpressionBase : IExpression
    {
        public ExpressionBase(Tag t)
        {
            if (t.Properties.ContainsKey(MODS))
                Mods = t.Properties[MODS].Value.ToString();
            if (t.Properties.ContainsKey(SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[SEARCH].Value)
                    Search.Add(val.Value.ToString());
            }
        }

        public const string MODS = "m";
        public const string SEARCH = "s";

        public List<string> Search { get; set; } = new List<string>();
        public string Mods { get; set; } = string.Empty;

        public abstract bool Test(string s);

        public static IExpression ToExpression(Tag t)
        {
            switch (t.Name)
            {
                case ContainsExpression.NAME:
                    return new ContainsExpression(t);

                case StartsWithExpression.NAME:
                    return new StartsWithExpression(t);

                case EndsWithExpression.NAME:
                    return new EndsWithExpression(t);

                case RegexExpression.NAME:
                    return new RegexExpression(t);

                case AndExpression.NAME:
                    return new AndExpression(t);

                case OrExpression.NAME:
                    return new OrExpression(t);

                case CountExpression.NAME:
                    return new CountExpression(t);

                case LookExpression.NAME:
                    return new LookExpression(t);
            }

            throw new ArgumentException("Unsupported Expression Tag", nameof(t));
        }
    }
}
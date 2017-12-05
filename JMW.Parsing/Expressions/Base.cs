using System;
using System.Collections.Generic;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public abstract class Base : IExpression
    {
        public Base(List<string> search, string mods)
        {
            Mods = mods;
            Search = search;
        }

        public Base(Tag t)
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
                case Contains.NAME:
                    return new Contains(t);

                case StartsWith.NAME:
                    return new StartsWith(t);

                case EndsWith.NAME:
                    return new EndsWith(t);

                case Regex.NAME:
                    return new Regex(t);

                case And.NAME:
                    return new And(t);

                case Or.NAME:
                    return new Or(t);

                case Count.NAME:
                    return new Count(t);

                case Look.NAME:
                    return new Look(t);
            }

            throw new ArgumentException("Unsupported Expression Tag", nameof(t));
        }
    }
}
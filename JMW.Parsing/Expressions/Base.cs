using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public abstract class Base : IExpression
    {
        public Base(List<string> search, string mods)
        {
            Search.Mods = mods;
            Search.Query = search;
        }

        public Base(Tag t)
        {
            if (t.Properties.ContainsKey(Search.SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[Search.SEARCH].Value)
                    Search.Query.Add(val.Value.ToString());

                if (t.Properties[Search.SEARCH].Properties.ContainsKey(Search.MODS))
                    Search.Mods = t.Properties[Search.SEARCH].Properties[Search.MODS].Value.ToString();
            }
        }

        public Search Search { get; set; } = new Search();

        public abstract bool Test(string s);

        /// <summary>
        /// Uses a function to test against a provided string.
        /// </summary>
        /// <param name="s">String to search against</param>
        /// <param name="func">Function that takes the String to test against and a value to test for and returns a boolean.</param>
        public bool Test(string s, Func<string, string, bool> func)
        {
            bool v;

            if (Search.Mods.Contains(Constants.CASE_INSENSITIVE)) // run case insensitive
                v = Search.Query.Any(sr => func(s.ToLower(), sr.ToLower()));
            else
                v = Search.Query.Any(sr => func(s, sr));

            return Search.Mods.Contains(Constants.NEGATE) ? !v : v;
        }

        public static IExpression ToExpression(Tag t)
        {
            switch (t.Name)
            {
                case Contains.NAME:
                    return new Contains(t);

                case StartsWith.NAME1:
                case StartsWith.NAME2:
                    return new StartsWith(t);

                case EndsWith.NAME1:
                case EndsWith.NAME2:
                    return new EndsWith(t);

                case Regex.NAME1:
                case Regex.NAME2:
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
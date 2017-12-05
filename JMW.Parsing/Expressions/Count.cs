using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.String;
using JMW.Parsing.Compile;
using JMW.Types;

namespace JMW.Parsing.Expressions
{
    public class Count : IExpression
    {
        public const string NAME = "count";
        public const string SEARCH = "s";
        public const string RNG = "rng";

        public List<string> Search { get; set; } = new List<string>();
        public IntegerRangeCollection Rng { get; set; }

        public Count(Tag t)
        {
            if (t.Properties.ContainsKey(SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[SEARCH].Value)
                    Search.Add(val.Value.ToString());
            }
            if (t.Properties.ContainsKey(RNG))
                Rng = new IntegerRangeCollection(t.Properties[RNG].Value.ToString());
            else
                throw new ArgumentException("Required Property Missing: " + RNG);
        }

        public bool Test(string s)
        {
            var c = 0;
            var t = s;
            while (Search.Any(t.Contains))
            {
                t = t.ParseAfterIndexOf_PlusLength(Search.ToArray());
            }
            return Rng.Contains(c);
        }
    }
}
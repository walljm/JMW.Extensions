using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Collections;
using JMW.Extensions.String;
using JMW.Parsing.Compile;
using JMW.Types;

namespace JMW.Parsing.Expressions
{
    public class Count : IExpression
    {
        public const string NAME = "count";
        public const string RNG = "rng";

        public Search Search { get; set; } = new Search();
        public IntegerRangeCollection Rng { get; set; }

        public Count(Tag t)
        {
            if (t.Properties.ContainsKey(Search.SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[Search.SEARCH].Value)
                    Search.Query.Add(val.Value.ToString());

                if (t.Properties[Search.SEARCH].Properties.ContainsKey(Search.MODS))
                    Search.Mods = t.Properties[Search.SEARCH].Properties[Search.MODS].Value.ToString();
            }
            else
                throw new ArgumentException("Required Property Missing: " + Search.SEARCH);

            if (t.Properties.ContainsKey(RNG))
                Rng = new IntegerRangeCollection(t.Properties[RNG].Value.ToString());
            else
                throw new ArgumentException("Required Property Missing: " + RNG);
        }

        public Count(List<string> search, IntegerRangeCollection rng, string mods)
        {
            Rng = rng;
            Search.Query = search;
            Search.Mods = mods;
        }

        public bool Test(string s)
        {
            var i = Search.Mods.Contains(Constants.CASE_INSENSITIVE);
            var c = 0;
            var t = s;
            while (Search.Query.Any(sr =>
            {
                if (i) return t.ToLower().Contains(sr.ToLower());

                return t.Contains(sr);
            }))
            {
                c++;
                t = t.ParseAfterIndexOf_PlusLength(i, Search.Query.ToArray());
            }
            return Rng.Contains(c);
        }
    }
}
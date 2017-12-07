using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.String;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class Look : IExpression
    {
        public const string NAME = "look";
        public const string AHEAD = "a";
        public const string BEHIND = "b";

        public Search Search { get; set; } = new Search();
        public Search Ahead { get; set; } = new Search();
        public Search Behind { get; set; } = new Search();

        public Look(Tag t)
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

            if (t.Properties.ContainsKey(AHEAD))
            {
                foreach (var val in (Stack<Tag>)t.Properties[AHEAD].Value)
                    Ahead.Query.Add(val.Value.ToString());

                if (t.Properties[AHEAD].Properties.ContainsKey(Search.MODS))
                    Search.Mods = t.Properties[AHEAD].Properties[Search.MODS].Value.ToString();
            }
            if (t.Properties.ContainsKey(BEHIND))
            {
                foreach (var val in (Stack<Tag>)t.Properties[BEHIND].Value)
                    Behind.Query.Add(val.Value.ToString());

                if (t.Properties[BEHIND].Properties.ContainsKey(Search.MODS))
                    Search.Mods = t.Properties[BEHIND].Properties[Search.MODS].Value.ToString();
            }
        }

        public Look(List<string> search, string mods, List<string> ahead, string modsa, List<string> behind, string modsb)
        {
            Search.Query = search;
            Search.Mods = mods;
            Ahead.Query = ahead;
            Ahead.Mods = modsa;
            Behind.Query = behind;
            Behind.Mods = modsb;
        }

        public bool Test(string s)
        {
            var t = s;

            var i = Search.Mods.Contains(Constants.CASE_INSENSITIVE);

            // find case insensitively.
            bool f(string sch, string qry)
            {
                if (i) return sch.ToLower().Contains(qry.ToLower());
                return sch.Contains(qry);
            }

            if (!Search.Query.Any(sr => f(t, sr)))
                return false;

            if (Ahead.Query.Count > 0 && !Ahead.Query.Any(sr => f(t, sr)))
                return false;

            if (Behind.Query.Count > 0 && !Behind.Query.Any(sr => f(t, sr)))
                return false;

            if (Behind.Query.Count > 0)
                t = t.ParseToLastIndexOf(Behind.Query.ToArray());
            if (Ahead.Query.Count > 0)
                t = t.ParseAfterLastIndexOf_PlusLength(Ahead.Query.ToArray());

            var v = Search.Query.Any(sr => f(t, sr));
            return Search.Mods.Contains(Constants.NEGATE) ? !v : v;
        }
    }
}
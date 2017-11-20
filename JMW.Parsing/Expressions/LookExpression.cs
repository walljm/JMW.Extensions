using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.String;

namespace JMW.Parsing.Expressions
{
    public class LookExpression : IExpression
    {
        public const string NAME = "look";
        public const string SEARCH = "s";
        public const string AHEAD = "a";
        public const string BEHIND = "b";


        public List<string> Search { get; set; } = new List<string>();
        public List<string> Ahead { get; set; } = new List<string>();
        public List<string> Behind { get; set; } = new List<string>();

        public LookExpression(Tag t)
        {
            if (t.Properties.ContainsKey(SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[SEARCH].Value)
                    Search.Add(val.Value.ToString());
            }
            else
                throw new ArgumentException("Required Property Missing: " + SEARCH);

            if (t.Properties.ContainsKey(AHEAD))
            {
                foreach (var val in (Stack<Tag>)t.Properties[AHEAD].Value)
                    Ahead.Add(val.Value.ToString());
            }
            if (t.Properties.ContainsKey(BEHIND))
            {
                foreach (var val in (Stack<Tag>)t.Properties[BEHIND].Value)
                    Behind.Add(val.Value.ToString());
            }
        }

        public bool Test(string s)
        {
            var t = s;
            if (Search.Any(t.Contains) && Ahead.Any(t.Contains) && Behind.Any(t.Contains))
            {
                t = t.ParseAfterIndexOf_PlusLength(Behind.ToArray()).ParseToLastIndexOf(Ahead.ToArray());
                return Search.Any(t.Contains);
            }

            return false;
        }
    }
}
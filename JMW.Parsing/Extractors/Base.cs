using System;
using System.Collections.Generic;
using JMW.Extensions.Numbers;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public abstract class Base
    {
        public const string QUANTIFIER = "q";
        public const string INDEX = "i";

        public Base(Tag t)
        {
            if (t.Properties.ContainsKey(Search.SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[Search.SEARCH].Value)
                    Search.Query.Add(val.Value.ToString());

                if (t.Properties[Search.SEARCH].Properties.ContainsKey(Search.MODS))
                    Search.Mods = t.Properties[Search.SEARCH].Properties[Search.MODS].Value.ToString();
            }

            if (t.Properties.ContainsKey(QUANTIFIER))
            {
                var n = t.Properties[QUANTIFIER].Value.ToString().ToIntOrNeg1();
                if (n == -1)
                    throw new ParseException(INDEX + " must be a 32 bit integer.");
                Quantifier = n;
            }

            if (t.Properties.ContainsKey(INDEX))
            {
                var n = t.Properties[INDEX].Value.ToString().ToIntOrNeg1();
                if (n == -1)
                    throw new ParseException(INDEX + " must be a 32 bit integer.");
                Index = n;
            }
        }

        public Base(List<string> search, string mods, int q, int idx)
        {
            Search.Query = search;
            Search.Mods = mods;
            Quantifier = q;
            Index = idx;
        }

        public Search Search { get; set; } = new Search();
        public int Quantifier { get; set; } = -1;
        public int Index { get; set; } = -1;

        public abstract string Parse(string s);

        public static Base InToParser(Tag t)
        {
            switch (t.Name)
            {
                case To.NAME:
                    return new To(t);

                case After.NAME:
                    return new After(t);

                case Split.NAME:
                    return new Split(t);
            }

            throw new ArgumentException("Unsupported Expression Tag", nameof(t));
        }
    }
}
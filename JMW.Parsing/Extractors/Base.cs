using System;
using System.Collections.Generic;
using JMW.Extensions.Numbers;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public abstract class Base
    {
        public const string SEARCH = "s";
        public const string MODS = "m";
        public const string QUANTIFIER = "q";
        public const string INDEX = "i";

        public Base(Tag t)
        {
            if (t.Properties.ContainsKey(MODS))
                Mods = t.Properties[MODS].Value.ToString();

            if (t.Properties.ContainsKey(SEARCH))
            {
                foreach (var val in (Stack<Tag>)t.Properties[SEARCH].Value)
                    Search.Add(val.Value.ToString());
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

        public List<string> Search { get; set; } = new List<string>();
        public string Mods { get; set; } = string.Empty;
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
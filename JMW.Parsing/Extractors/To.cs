using JMW.Extensions.String;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public class To : Base
    {
        public const string NAME = "to";

        public override string Parse(string s)
        {
            var i = Mods.Contains("i"); // case insensitive
            var t = Mods.Contains("t"); // trim the output

            if (Mods.Contains("w")) // include search item
            {
                if (Mods.Contains("l")) // make it the last item
                    return t ? s.ParseToLastIndexOf_PlusLength(i, Search.ToArray()).Trim()
                             : s.ParseToLastIndexOf_PlusLength(i, Search.ToArray());

                if (Quantifier > 0) // parse to a specific number of the item
                    return t ? s.ParseToDesignatedIndexOf_PlusLength(Quantifier, i, Search.ToArray()).Trim()
                             : s.ParseToDesignatedIndexOf_PlusLength(Quantifier, i, Search.ToArray());

                return t ? s.ParseToIndexOf_PlusLength(i, Search.ToArray()).Trim()
                         : s.ParseToIndexOf_PlusLength(i, Search.ToArray());
            }

            return t ? s.ParseToIndexOf(i, Search.ToArray()).Trim()
                     : s.ParseToIndexOf(i, Search.ToArray());
        }

        public To(Tag t) : base(t)
        {
        }
    }
}
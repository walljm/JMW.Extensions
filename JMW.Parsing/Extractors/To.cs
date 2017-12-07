using System.Collections.Generic;
using JMW.Extensions.String;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public class To : Base
    {
        public const string NAME = "to";

        public override string Parse(string s)
        {
            var i = Search.Mods.Contains(Constants.CASE_INSENSITIVE); // case insensitive
            var t = Search.Mods.Contains(Constants.TRIM); // trim the output

            if (Search.Mods.Contains(Constants.WITH_VALUE)) // include search item
            {
                if (Quantifier > 0) // parse to a specific number of the item
                    return t ? s.ParseToDesignatedIndexOf_PlusLength(Quantifier, i, Search.Query.ToArray()).Trim()
                             : s.ParseToDesignatedIndexOf_PlusLength(Quantifier, i, Search.Query.ToArray());

                if (Search.Mods.Contains(Constants.LAST)) // make it the last item
                    return t ? s.ParseToLastIndexOf_PlusLength(i, Search.Query.ToArray()).Trim()
                        : s.ParseToLastIndexOf_PlusLength(i, Search.Query.ToArray());

                return t ? s.ParseToIndexOf_PlusLength(i, Search.Query.ToArray()).Trim()
                         : s.ParseToIndexOf_PlusLength(i, Search.Query.ToArray());
            }

            return t ? s.ParseToIndexOf(i, Search.Query.ToArray()).Trim()
                     : s.ParseToIndexOf(i, Search.Query.ToArray());
        }

        public To(Tag t) : base(t)
        {
        }

        public To(List<string> search, string mods, int q) : base(search, mods, q, -1)
        {
        }
    }
}
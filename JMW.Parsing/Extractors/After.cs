using System.Collections.Generic;
using JMW.Extensions.String;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public class After : Base
    {
        public const string NAME = "after";

        public override string Parse(string s)
        {
            var i = Search.Mods.Contains(Constants.CASE_INSENSITIVE); // case insensitive
            var t = Search.Mods.Contains(Constants.TRIM); // trim the output

            if (Search.Mods.Contains(Constants.WITH_VALUE)) // exclude the search item
            {
                if (Quantifier > -1) // after a specific item
                    return t ? s.ParseAfterDesignatedIndexOf_PlusLength(Quantifier, i, Search.Query.ToArray()).Trim()
                             : s.ParseAfterDesignatedIndexOf_PlusLength(Quantifier, i, Search.Query.ToArray());

                if (Search.Mods.Contains(Constants.LAST)) // after the last item
                    return t ? s.ParseAfterLastIndexOf_PlusLength(i, Search.Query.ToArray()).Trim()
                             : s.ParseAfterLastIndexOf_PlusLength(i, Search.Query.ToArray());

                return t ? s.ParseAfterIndexOf_PlusLength(i, Search.Query.ToArray()).Trim()
                         : s.ParseAfterIndexOf_PlusLength(i, Search.Query.ToArray());
            }

            return t ? s.ParseAfterIndexOf(i, Search.Query.ToArray()).Trim()
                     : s.ParseAfterIndexOf(i, Search.Query.ToArray());
        }

        public After(Tag t) : base(t)
        {
        }

        public After(List<string> search, string mods, int q) : base(search, mods, q, -1)
        {
        }
    }
}
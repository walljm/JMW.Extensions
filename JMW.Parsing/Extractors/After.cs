using JMW.Extensions.String;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public class After : Base
    {
        public const string NAME = "after";

        public override string Parse(string s)
        {
            var i = Mods.Contains("i"); // case insensitive
            var t = Mods.Contains("t"); // trim the output

            if (Mods.Contains("w")) // exclude the search item
            {
                if (Mods.Contains("l")) // after the last item
                    return t ? s.ParseAfterLastIndexOf_PlusLength(i, Search.ToArray()).Trim()
                             : s.ParseAfterLastIndexOf_PlusLength(i, Search.ToArray());

                if (Quantifier > 0) // after a specific item
                    return t ? s.ParseAfterDesignatedIndexOf_PlusLength(Quantifier, i, Search.ToArray()).Trim()
                             : s.ParseAfterDesignatedIndexOf_PlusLength(Quantifier, i, Search.ToArray());

                return t ? s.ParseAfterIndexOf_PlusLength(i, Search.ToArray()).Trim()
                         : s.ParseAfterIndexOf_PlusLength(i, Search.ToArray());
            }

            return t ? s.ParseAfterIndexOf(i, Search.ToArray()).Trim()
                     : s.ParseAfterIndexOf(i, Search.ToArray());
        }

        public After(Tag t) : base(t)
        {
        }
    }
}
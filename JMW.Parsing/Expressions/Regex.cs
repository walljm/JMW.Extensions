using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Expressions
{
    public class Regex : Base
    {
        public const string NAME1 = "regex";
        public const string NAME2 = "reg";

        public Regex(Tag t) : base(t)
        {
        }

        public Regex(List<string> search, string mods) : base(search, mods)
        {
        }

        public override bool Test(string s)
        {
            bool v;
            if (Search.Mods.Contains(Constants.CASE_INSENSITIVE))
                v = Search.Query.Any(sr => System.Text.RegularExpressions.Regex.IsMatch(s, sr, RegexOptions.IgnoreCase));
            else
                v = Search.Query.Any(sr => System.Text.RegularExpressions.Regex.IsMatch(s, sr));

            return Search.Mods.Contains(Constants.NEGATE) ? !v : v;
        }
    }
}
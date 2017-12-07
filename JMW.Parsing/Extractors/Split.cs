using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Extractors
{
    public class Split : Base
    {
        public const string NAME = "split";

        public override string Parse(string s)
        {
            if (Index < 0)
                throw new ArgumentException("Required Attribute Missing: " + INDEX);

            var r = Search.Mods.Contains(Constants.REMOVE_EMPTY_ENTRIES); // remove the empty entries
            var t = Search.Mods.Contains(Constants.TRIM); // trim the output

            string[] fields;

            if (Search.Mods.Contains(Constants.CASE_INSENSITIVE)) // case insensitive
                fields = s.ToLower().Split(Search.Query.Select(o => o.ToLower()).ToArray(), r ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            else
                fields = s.Split(Search.Query.ToArray(), r ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

            if (fields.Length > Index)
            {
                return t
                    ? fields[Index].Trim()
                    : fields[Index];
            }

            throw new IndexOutOfRangeException();
        }

        public Split(Tag t) : base(t)
        {
        }

        public Split(List<string> search, string mods, int idx) : base(search, mods, -1, idx)
        {
        }
    }
}
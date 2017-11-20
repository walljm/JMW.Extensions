using System;
using System.Linq;

namespace JMW.Parsing.Parsers
{
    public class SplitParser : ParserBase
    {
        public const string NAME = "split";

        public override string Parse(string s)
        {
            if (Index < 0)
                throw new ArgumentException("Required Attribute Missing: " + INDEX);

            var r = Mods.Contains("r"); // remove the empty entries
            var t = Mods.Contains("t"); // trim the output

            string[] fields;

            if (Mods.Contains("i")) // case insensitive
                fields = s.ToLower().Split(Search.Select(o=> o.ToLower()).ToArray(), r ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            else
                fields = s.Split(Search.ToArray(), r ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

            if (fields.Length > Index)
            {
                return t
                    ? fields[Index].Trim()
                    : fields[Index];
            }

            throw new IndexOutOfRangeException();
        }

        public SplitParser(Tag t) : base(t)
        {
        }
    }
}
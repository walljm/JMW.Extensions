using System.Collections.Generic;
using System.IO;
using System.Text;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Handlers
{
    public class Property : IProperty
    {
        public const string NAME = "name";
        public const string PARSERS = "parsers";
        public const string LINE = "line";
        public const string SPLIT = "split";

        public string Name { get; set; }
        public Stack<Extractors.Base> Extractors { get; set; } = new Stack<Extractors.Base>();
        public Expressions.IExpression Line { get; set; }
        public string Split { get; set; }

        public Property(Tag t)
        {
            if (t.Properties.ContainsKey(NAME))
                Name = t.Properties[NAME].Value.ToString();
            else
                throw new ParseException("Required Attribute Missing: " + NAME);

            if (t.Properties.ContainsKey(PARSERS))
            {
                var parsers = (Stack<Tag>)t.Properties[PARSERS].Value;
                foreach (var p in parsers)
                    Extractors.Push(Parsing.Extractors.Base.InToParser(p));
            }
            else
                throw new ParseException("Required Attribute Missing: " + PARSERS);

            if (t.Properties.ContainsKey(LINE))
            {
                Line = Expressions.Base.ToExpression(t.Properties[LINE]);
            }
            else
                throw new ParseException("Required Attribute Missing: " + LINE);

            if (t.Properties.ContainsKey(SPLIT))
                Split = t.Properties[SPLIT].Value.ToString();
        }

        public object Parse(StreamReader reader)
        {
            if (Extractors.Count > 0)
            {
                if (Split != null)
                {
                    var c = 0;
                    while (c != -1)
                    {
                        var l = Split.Length;
                        var test = string.Empty;
                        var line = string.Empty;
                        var sb = new StringBuilder();
                        while ((c = reader.Read()) != -1)
                        {
                            test += (char)c;
                            sb.Append((char)c);

                            if (test.Length > l)
                            {
                                test = test.Substring(test.Length - l);
                            }
                            if (test == Split)
                            {
                                line = sb.ToString(0, sb.Length - test.Length);
                                break;
                            }
                        }

                        // if its the last line, you need to set it.
                        if (c == -1)
                        {
                            if (test == Split)
                                line = sb.ToString(test.Length, sb.Length - test.Length);
                            else
                                line = sb.ToString();
                        }

                        if (!Line.Test(line))
                            continue;

                        var r = line;
                        foreach (var parser in Extractors)
                            r = parser.Parse(r);
                        return r;
                    }
                }
                else
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Line.Test(line))
                            continue;

                        var r = line;
                        foreach (var parser in Extractors)
                            r = parser.Parse(r);
                        return r;
                    }
                }
            }

            return string.Empty;
        }
    }
}
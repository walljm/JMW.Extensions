using System.Collections.Generic;
using System.IO;
using System.Text;
using JMW.Parsing.Expressions;
using JMW.Parsing.Parsers;

namespace JMW.Parsing.Handlers
{
    public class Property
    {
        public const string NAME = "name";
        public const string PARSERS = "parsers";
        public const string LINE = "line";
        public const string SPLIT = "split";

        public string Name { get; set; }
        public Stack<ParserBase> Parsers { get; set; } = new Stack<ParserBase>();
        public IExpression Line { get; set; }
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
                    Parsers.Push(ParserBase.InToParser(p));
            }
            else
                throw new ParseException("Required Attribute Missing: " + PARSERS);

            if (t.Properties.ContainsKey(LINE))
            {
                Line = ExpressionBase.ToExpression(t.Properties[LINE]);
            }
            else
                throw new ParseException("Required Attribute Missing: " + LINE);

            if (t.Properties.ContainsKey(SPLIT))
                Split = t.Properties[SPLIT].Value.ToString();
        }

        public object Eval(TextReader reader)
        {
            if (Parsers.Count > 0)
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

                            if (test.Length > l)
                            {
                                test = test.Substring(test.Length - l);
                            }
                            if (test == Split)
                            {
                                line = sb.ToString(test.Length, sb.Length - test.Length);
                                break;
                            }
                            sb.Append((char)c);
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
                        foreach (var parser in Parsers)
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
                        foreach (var parser in Parsers)
                            r = parser.Parse(r);
                        return r;
                    }
                }
            }

            return "";
        }
    }
}
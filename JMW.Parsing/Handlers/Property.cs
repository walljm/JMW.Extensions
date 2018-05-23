using System.Collections.Generic;
using System.IO;
using System.Text;
using JMW.Parsing.Compile;

namespace JMW.Parsing.Handlers
{
    public class Property : IProperty
    {
        public const string NAME = "prop";
        public const string A_NAME = "name";
        public const string A_PARSERS = "parsers";
        public const string A_LINE = "line";
        public const string A_SPLIT = "split";

        public string Name { get; set; }
        public Stack<Extractors.Base> Extractors { get; set; } = new Stack<Extractors.Base>();
        public Expressions.IExpression Line { get; set; }
        public string Split { get; set; }

        public Property(Tag token)
        {
            if (token.Properties.ContainsKey(A_NAME))
                Name = token.Properties[A_NAME].Value.ToString();
            else
                throw new ParseException("Required Attribute Missing: " + A_NAME);

            if (token.Properties.ContainsKey(A_PARSERS))
            {
                var parsers = (Stack<Tag>)token.Properties[A_PARSERS].Value;
                foreach (var p in parsers)
                    Extractors.Push(Parsing.Extractors.Base.ToExtractor(p));
            }
            else
                throw new ParseException("Required Attribute Missing: " + A_PARSERS);

            if (token.Properties.ContainsKey(A_LINE))
            {
                Line = Expressions.Base.ToExpression(token.Properties[A_LINE]);
            }
            
            if (token.Properties.ContainsKey(A_SPLIT))
                Split = token.Properties[A_SPLIT].Value.ToString();
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
                        if (Line != null && !Line.Test(line))
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
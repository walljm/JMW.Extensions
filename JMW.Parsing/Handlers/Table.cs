using System.Collections.Generic;
using System.IO;
using System.Linq;
using JMW.Extensions.Enumerable;
using JMW.Parsing.Expressions;

namespace JMW.Parsing.Handlers
{
    public class Table : ITagHandler
    {
        public const string NAME = "table";

        public const string A_INCLUDE = "include";
        public const string A_START = "start";
        public const string A_STOP = "stop";

        #region Implementation of ITagHandler

        public IEnumerable<object[]> Eval(Tag token, TextReader reader)
        {
            if (reader == null)
                yield break;

            var include = new Include();

            if (token.Properties.ContainsKey(A_INCLUDE))
            {
                include = new Include(token.Properties[A_INCLUDE]);
            }
            IExpression stop = null;
            if (token.Properties.ContainsKey(A_STOP))
                stop = ExpressionBase.ToExpression(token.Properties[A_STOP]);

            if (!token.Properties.ContainsKey(A_START))
                throw new ParseException("Missing property: " + A_START);

            // find first line that matches.
            var start = ExpressionBase.ToExpression(token.Properties[A_START]);

            // parse the paragraphs
            foreach (var lines in include.GetNextParagraph(reader, start, stop))
            {
                var record = new List<object>();

                if (lines.Count == 0 || !start.Test(lines.First()))
                    continue;

                // process the properties
                var props = (Stack<Tag>)token.Properties["props"].Value;
                var line = lines.ToDelimitedString("\n");
                foreach (var p in props)
                {
                    if (p.Name == Paragraph.NAME)
                    {
                        var para = new Paragraph();
                        record.Add(para.Eval(p, new StringReader(line)));
                    }
                    else
                    {
                        var prop = new Property(p);
                        record.Add(prop.Eval(new StringReader(line)));
                    }
                }
                yield return record.ToArray();
            }
        }

        public void Validate(Tag tag, Token token)
        {
        }

        public string Name { get; } = NAME;

        #endregion Implementation of ITagHandler
    }
}
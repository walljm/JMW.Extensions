using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JMW.Extensions.Enumerable;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;

namespace JMW.Parsing.Handlers
{
    public class Paragraph : Base, IParser, IProperty
    {
        public const string NAME = "para";

        public const string A_INCLUDE = "include";
        public const string A_START = "start";
        public const string A_STOP = "stop";
        public const string A_PROPS = "props";
        public const string A_SPLIT = "split";

        public string Name { get; } = NAME;

        public void Validate(Tag tag, Token token)
        {
        }

        private string _split;

        private Include _include = new Include();
        private IExpression _stop = null;
        private IExpression _start = null;

        public Paragraph(Tag token)
        {
            if (!token.Properties.ContainsKey(A_START))
                throw new ParseException("Missing property: " + A_START);
            if (!token.Properties.ContainsKey(A_PROPS))
                throw new ParseException("Missing property: " + A_PROPS);

            if (token.Properties.ContainsKey(A_INCLUDE))
                _include = new Include(token.Properties[A_INCLUDE]);

            if (token.Properties.ContainsKey(A_STOP))
                _stop = Expressions.Base.ToExpression(token.Properties[A_STOP]);

            if (token.Properties.ContainsKey(A_SPLIT))
                _split = token.Properties[A_SPLIT].Value.ToString();

            // find first line that matches.
            _start = Expressions.Base.ToExpression(token.Properties[A_START]);

            var props = (Stack<Tag>)token.Properties[A_PROPS].Value;
            foreach (var p in props)
            {
                if (p.Name == NAME)
                    _props.Add(new Paragraph(p));
                else
                    _props.Add(new Property(p));
            }
        }

        public override IEnumerable<object[]> Parse(StreamReader reader)
        {
            if (reader == null)
                yield break;

            var paras = _include.GetParagraphs(reader, _start, _stop);

            // parse the paragraphs
            foreach (var lines in paras)
            {
                var record = new List<object>();

                if (lines.Count == 0 || !_start.Test(lines.First()))
                    continue;

                // process the properties
                var line = lines.ToDelimitedString("\n");
                foreach (var p in _props)
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                    using (var sr = new StreamReader(ms))
                    {
                        record.Add(p.Parse(sr));
                    }
                }
                yield return record.ToArray();
            }
        }

        object IProperty.Parse(StreamReader reader)
        {
            return Parse(reader);
        }
    }
}
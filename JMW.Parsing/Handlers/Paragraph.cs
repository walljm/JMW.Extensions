using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JMW.Extensions.Enumerable;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;
using JMW.Parsing.IO;

namespace JMW.Parsing.Handlers
{
    public class Paragraph : Base, IParser, IProperty
    {
        public const string NAME = "para";

        public const string A_INCLUDE = "include";
        public const string A_START = "start";
        public const string A_STOP = "stop";
        public const string A_PROPS = "props";

        public string Name { get; } = NAME;

        private Include _include = new Include(null, null);
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

            var paras = GetParagraphs(reader, _start, _stop);

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

        public IEnumerable<List<string>> GetParagraphs(StreamReader reader, IExpression para_start, IExpression para_stop)
        {
            var sr = new SectionReader(reader, _include);

            var paragraph = new List<string>();
            var started = false;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (paragraph.Count > 0 && ((para_stop != null && para_stop.Test(line)) || para_start.Test(line)))
                {
                    yield return paragraph;
                    paragraph = new List<string>();
                }

                if (!started && _start != null && !_start.Test(line))
                    continue;

                started = true;

                if (_stop != null && _stop.Test(line))
                {
                    started = false;
                    continue;
                }

                paragraph.Add(line);
            }

            // return the last paragraph.
            yield return paragraph;
        }

        object IProperty.Parse(StreamReader reader)
        {
            return Parse(reader);
        }
    }
}
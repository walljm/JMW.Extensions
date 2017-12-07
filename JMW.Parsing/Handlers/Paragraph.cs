using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JMW.Extensions.Enumerable;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;

namespace JMW.Parsing.Handlers
{
    public class Paragraph : IParser, IProperty
    {
        public const string NAME = "para";

        public const string A_INCLUDE = "include";
        public const string A_START = "start";
        public const string A_STOP = "stop";

        public void Validate(Tag tag, Token token)
        {
        }

        public string Name { get; } = NAME;

        private Include _include = new Include();
        private IExpression _stop = null;
        private IExpression _start = null;
        private List<IProperty> _props = new List<IProperty>();

        public Paragraph(Tag token)
        {
            if (!token.Properties.ContainsKey(A_START))
                throw new ParseException("Missing property: " + A_START);

            if (token.Properties.ContainsKey(A_INCLUDE))
                _include = new Include(token.Properties[A_INCLUDE]);

            if (token.Properties.ContainsKey(A_STOP))
                _stop = Base.ToExpression(token.Properties[A_STOP]);

            // find first line that matches.
            _start = Base.ToExpression(token.Properties[A_START]);

            var props = (Stack<Tag>)token.Properties["props"].Value;
            foreach (var p in props)
            {
                if (p.Name == NAME)
                    _props.Add(new Paragraph(p));
                else
                    _props.Add(new Property(p));
            }
        }

        public IEnumerable<object[]> Parse(StreamReader reader)
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

        public IEnumerable<object[]> Parse(string text)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var sr = new StreamReader(ms))
            {
                foreach (var o in Parse(sr))
                    yield return o;
            }
        }

        public IEnumerable<Dictionary<string, object>> ParseNamed(string text)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var sr = new StreamReader(ms))
            {
                foreach (var o in Parse(sr))
                {
                    var i = 0;
                    var dict = new Dictionary<string, object>();
                    foreach (var p in _props)
                    {
                        dict.Add(p.Name, o[i]);
                        i++;
                    }
                    yield return dict;
                }
            }
        }

        object IProperty.Parse(StreamReader reader)
        {
            return Parse(reader);
        }
    }
}
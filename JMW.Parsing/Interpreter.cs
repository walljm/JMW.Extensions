using System.Collections.Generic;
using System.IO;
using JMW.Parsing.Handlers;

namespace JMW.Parsing
{
    public class Interpreter
    {
        private Parser _parser = new Parser();
        private TextReader _reader;

        public Interpreter(TextReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Parse the template in the input, then interpret the tags.
        /// </summary>
        /// <param name="input">A template string</param>
        public IEnumerable<object[]> Eval(string input)
        {
            var tags = _parser.Parse(input);

            foreach (var tag in tags)
            {
                switch (tag.Name)
                {
                    case Paragraph.NAME:
                        {
                            var p = new Paragraph();
                            foreach (var o in p.Eval(tag, _reader))
                                yield return o;
                        }
                        break;

                    case Table.NAME:
                        {
                            var p = new Table();
                            foreach (var o in p.Eval(tag, _reader))
                                yield return o;
                        }
                        break;
                }
            }
        }
    }
}
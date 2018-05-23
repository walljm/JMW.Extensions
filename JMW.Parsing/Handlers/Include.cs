using System.Collections.Generic;
using System.IO;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;

namespace JMW.Parsing.Handlers
{
    public class Include
    {
        public const string NAME = "include";
        public const string START = "start";
        public const string STOP = "stop";

        private IExpression _start;
        private IExpression _stop;

        public Include()
        {
        }

        public Include(IExpression start, IExpression stop)
        {
            _start = start;
            _stop = stop;
        }

        public Include(Tag t)
        {
            if (t.Properties.ContainsKey(START))
                _start = Expressions.Base.ToExpression(t.Properties[START]);
            if (t.Properties.ContainsKey(STOP))
                _stop = Expressions.Base.ToExpression(t.Properties[STOP]);
        }

        public IEnumerable<string> GetNextRow(StreamReader reader, IExpression row)
        {
            var started = false;
            var stopped = false;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (row != null && row.Test(line))
                {
                    yield return line;
                }

                if (!started && _start != null && !_start.Test(line))
                    continue;

                started = true;

                if (!stopped && _stop != null && _stop.Test(line))
                    break;
                stopped = true;
            }
        }

        public IEnumerable<List<string>> GetParagraphs(StreamReader reader, IExpression para_start, IExpression para_stop)
        {
            var paragraph = new List<string>();
            var started = false;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if ((para_stop != null && para_stop.Test(line)) || para_start.Test(line))
                {
                    yield return paragraph;
                    paragraph = new List<string>();
                }

                if (!started && _start != null && !_start.Test(line))
                    continue;

                started = true;

                if (_stop != null && _stop.Test(line))
                {
                    yield return paragraph;
                    break;
                }

                paragraph.Add(line);
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
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

        public Include(Tag t)
        {
            if (t.Properties.ContainsKey(START))
                _start = ExpressionBase.ToExpression(t.Properties[START]);
            if (t.Properties.ContainsKey(STOP))
                _stop = ExpressionBase.ToExpression(t.Properties[STOP]);
        }

        public IEnumerable<List<string>> GetNextParagraph(TextReader reader, IExpression para_start, IExpression para_stop)
        {
            var paragraph = new List<string>();
            var started = false;
            var stopped = false;
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

                if (!stopped && _stop != null && _stop.Test(line))
                    break;
                stopped = true;

                paragraph.Add(line);
            }
        }
    }
}
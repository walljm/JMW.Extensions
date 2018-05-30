using JMW.Extensions.Conversions;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;

namespace JMW.Parsing.Handlers
{
    public class Include
    {
        public const string NAME = "include";
        public const string START = "start";
        public const string STOP = "stop";
        public const string INCLUDESTART = "include_start";
        public const string INCLUDESTOP = "include_stop";

        public IExpression Start { get; }
        public IExpression Stop { get; }
        public bool IncludeStart { get; private set; } = false;
        public bool IncludeStop { get; private set; } = false;

        public Include(IExpression start, IExpression stop)
        {
            Start = start;
            Stop = stop;
        }

        public Include(Tag t)
        {
            if (t.Properties.ContainsKey(START))
                Start = Expressions.Base.ToExpression(t.Properties[START]);
            if (t.Properties.ContainsKey(STOP))
                Stop = Expressions.Base.ToExpression(t.Properties[STOP]);
            if (t.Properties.ContainsKey(INCLUDESTART))
                t.Properties[INCLUDESTART].Value.ToString().ToBoolean().IfSuccess(v => IncludeStart = v);
            if (t.Properties.ContainsKey(INCLUDESTOP))
                t.Properties[INCLUDESTOP].Value.ToString().ToBoolean().IfSuccess(v => IncludeStop = v);
        }
    }
}
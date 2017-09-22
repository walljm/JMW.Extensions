using System.Collections.Generic;

namespace JMW.Template.Tags
{
    /// <summary>
    /// This is the base class for column handlers.  This is not intended to be used by itself,
    /// as it has now way to get data from anything.  
    /// </summary>
    public abstract class ColumnBase : TagHandlerBase
    {
        public static HashSet<string> ALLOWED = new HashSet<string> { ATTR_EXP, ATTR_OCT };
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public override string TagName { get; }

        public override HashSet<string> ALLOWEDPROPS { get; } = ALLOWED;

        public const string ATTR_EXP = "exp";
        public const string ATTR_OCT = "oct";

        protected static void HandleColumn(Tag token, Interpreter interp, string data)
        {
            if (token.Properties.ContainsKey(ATTR_EXP))
            {
                var arguments = new List<string>();
                if (token.Properties.ContainsKey(ATTR_OCT))
                {
                    arguments.Add(TagHelpers.RetrieveOctet(data, token.Properties[ATTR_OCT]));
                    var octet_result = TagHelpers.EvaluateArithmeticExpression(token.Properties[ATTR_EXP], arguments);
                    interp.OutputStream.Write(TagHelpers.ReplaceOctet(data, token.Properties[ATTR_OCT], octet_result));
                }
                else
                {
                    arguments.Add(data);
                    interp.OutputStream.Write(TagHelpers.EvaluateExpression(token.Properties[ATTR_EXP], arguments));
                }
            }
            else
            {
                interp.OutputStream.Write(data);
            }
        }
    }
}
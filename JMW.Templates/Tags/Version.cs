using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace JMW.Template.Tags
{
    public class Version : TagHandlerBase
    {
        public const string TAG = "version";
        public override string TagName { get; } = TAG;
        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_PREFIX, ATTR_EXP };
        public const string ATTR_PREFIX = "prefix";
        public const string ATTR_EXP = "exp";

        public override void Handler(Tag token, Interpreter interp)
        {
            var prefix = @"Templating Engine Version ";

            try
            {
                if (token.Properties.ContainsKey(ATTR_PREFIX))
                    prefix = token.Properties[ATTR_PREFIX];

                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    var arguments = prefix + assembly.GetName().Version;

                    if (token.Properties.ContainsKey(ATTR_EXP))
                    {
                        var value = TagHelpers.EvaluateExpression(token.Properties[ATTR_EXP], new List<string> { arguments }).ToString(CultureInfo.InvariantCulture);
                        interp.OutputStream.Write(value);
                    }
                    else
                        interp.OutputStream.Write(arguments);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to interpret tag: " + token.Name + " with prefix: " + prefix + ". Error Message: " + ex.Message, ex);
            }
        }
    }
}
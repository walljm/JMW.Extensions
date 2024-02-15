using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.String;

namespace JMW.Template.Tags
{
    public class Transform : TagHandlerBase
    {
        public const string TAG = "transform";
        public const string TAGALT1 = "trans";
        public const string TAGALT2 = "tr";

        public override string TagName { get; } = TAG;
        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string>
        {
            ATTR_COLUMN,
            ATTR_EXP
        };

        public const string ATTR_EXP = "exp";
        public const string ATTR_COLUMN = "columns";

        private Dictionary<string, ValueRetriever> _retrievers = new();

        public Transform()
        {
        }

        public Transform(Dictionary<string, ValueRetriever> retrievers)
        {
            _retrievers = retrievers;
        }

        public override void Handler(Tag token, Interpreter interp)
        {
            try
            {
                if (token.Properties.ContainsKey(ATTR_EXP))
                {
                    var values = new List<string>();
                    var columns = new List<string>();

                    if (token.Properties.ContainsKey(ATTR_COLUMN))
                    {
                        columns = token.Properties[ATTR_COLUMN].Split(new[] { "," }, StringSplitOptions.None).ToList();
                    }

                    foreach (var column in columns)
                    {
                        if (!string.IsNullOrWhiteSpace(column))
                        {
                            var retriever = column.ParseToLastIndexOf(":").Trim();
                            var col = column.ParseAfterLastIndexOf_PlusLength(":").Trim();
                            values.Add(_retrievers[retriever.ToLower()](col.ToLower()));
                        }
                    }

                    var val = TagHelpers.EvaluateExpression(token.Properties[ATTR_EXP], values);
                    interp.OutputStream.Write(val);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to interpret tag: " + token.Name + ". Error Message: " + ex.Message, ex);
            }
        }

        public static bool IsTransform(Tag tag)
        {
            return tag.TagType == TagTypes.Tag && (tag.Name == TAG || tag.Name == TAGALT1 || tag.Name == TAGALT2);
        }
    }
}
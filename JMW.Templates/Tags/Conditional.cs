﻿using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Extensions.Enumerable;
using JMW.Extensions.String;

namespace JMW.Template.Tags
{
    public class Conditional : TagHandlerBase
    {
        public const string TAG = "if";
        public override string TagName { get; } = TAG;

        public const string ATTR_EXP = "exp";
        public const string ATTR_OCT = "oct";
        public const string ATTR_COLUMN = "column";
        public const string ATTR_VALUE = "value";
        public const string ATTR_TYPE = "type";

        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_COLUMN, ATTR_VALUE, ATTR_TYPE, ATTR_EXP, ATTR_OCT };
        public HashSet<string> ALLOWEDPROPVALUES { get; } = new HashSet<string>
        {
            "equals", "notequals",
            "contains", "notcontains",
            "startswith", "notstartswith",
            "endswith", "notendswith",
            "eq", "neq",
            "cnt", "ncnt",
            "strt", "nstrt",
            "ends", "nends"
        };

        private Dictionary<string, ValueRetriever> _retrievers = new();
        private static readonly string[] separator = [","];

        public Conditional()
        {
        }

        public Conditional(Dictionary<string, ValueRetriever> retrievers)
        {
            _retrievers = retrievers;
        }

        public override void Handler(Tag token, Interpreter interp)
        {
            var condition_met = false;

            if (token.Properties.ContainsKey(ATTR_EXP))
            {
                var values = new List<string>();
                var columns = new List<string>();

                if (token.Properties.ContainsKey(ATTR_COLUMN))
                {
                    columns = token.Properties[ATTR_COLUMN].Split(separator, StringSplitOptions.None).ToList();
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

                if (token.Properties.ContainsKey(ATTR_OCT))
                {
                    if (values.Count != 1)
                    {
                        throw new Exception(
                            "The '" + ATTR_OCT + "' attribute is currently incompatible with multiple arguments for the '" + TAG + "' tag.");
                    }

                    values[0] = TagHelpers.RetrieveOctet(values[0], token.Properties[ATTR_OCT]);
                }

                condition_met = TagHelpers.EvaluateBooleanExpression(token.Properties[ATTR_EXP], values);
            }
            else if (token.Properties.ContainsKey(ATTR_VALUE) && token.Properties.ContainsKey(ATTR_TYPE))
            {
                // the column will always have a ':' to split on, as we alias default tables with a default table name.
                var split_data = token.Properties[ATTR_COLUMN].Split(':');
                var sheetID = split_data[0];
                var columnID = split_data[1];

                var value = token.Properties[ATTR_VALUE];
                var operatorID = token.Properties[ATTR_TYPE].ToLower();
                var actualValue = _retrievers[sheetID.ToLower()](columnID);

                switch (operatorID)
                {
                    case "eq":
                    case "equals":
                        {
                            if (actualValue == value)
                                condition_met = true;
                            break;
                        }
                    case "neq":
                    case "notequals":
                        {
                            if (actualValue != value)
                                condition_met = true;
                            break;
                        }
                    case "cnt":
                    case "contains":
                        {
                            if (actualValue.Contains(value))
                                condition_met = true;
                            break;
                        }
                    case "ncnt":
                    case "notcontains":
                        {
                            if (!actualValue.Contains(value))
                                condition_met = true;
                            break;
                        }
                    case "strt":
                    case "startswith":
                        {
                            if (actualValue.StartsWith(value))
                                condition_met = true;
                            break;
                        }
                    case "nstrt":
                    case "notstartswith":
                        {
                            if (!actualValue.StartsWith(value))
                                condition_met = true;
                            break;
                        }
                    case "ends":
                    case "endswith":
                        {
                            if (actualValue.EndsWith(value))
                                condition_met = true;
                            break;
                        }
                    case "nends":
                    case "notendswith":
                        {
                            if (!actualValue.EndsWith(value))
                                condition_met = true;
                            break;
                        }
                }
            }

            if (condition_met)
            {
                interp.Eval(token.Children);
            }
        }

        public override void Validate(Tag tag, Token token)
        {
            TagHelpers.CheckAllowedAttributes(tag, ALLOWEDPROPS, token);

            if (tag.Properties.ContainsKey(ATTR_TYPE) && !tag.Properties.ContainsKey(ATTR_VALUE))
                throw new ParseException("Missing attribute: '" + ATTR_VALUE + "'", token);

            if (!tag.Properties.ContainsKey(ATTR_TYPE) && tag.Properties.ContainsKey(ATTR_VALUE))
                throw new ParseException("Missing attribute: '" + ATTR_TYPE + "'", token);

            if (!tag.Properties.ContainsKey(ATTR_EXP) && !tag.Properties.ContainsKey(ATTR_TYPE) && !tag.Properties.ContainsKey(ATTR_VALUE))
                throw new ParseException("Missing attribute.  You must have either a '" + ATTR_TYPE + "' and a '" + ATTR_VALUE + "' attribute or an '" + ATTR_EXP + "' attribute.", token);

            if (tag.Properties.ContainsKey(ATTR_TYPE))
            {
                var type = tag.Properties[ATTR_TYPE].ToLower();

                if (!ALLOWEDPROPVALUES.Contains(type))
                {
                    throw new ParseException("Invalid property value for 'type' property on '" + TAG + "' tag. Allowed values: '" + ALLOWEDPROPVALUES.ToDelimitedString("', '") + "'.", token);
                }
            }
        }

        public static bool IsConditional(Tag tag)
        {
            return tag.TagType == TagTypes.Tag && tag.Name == TAG;
        }
    }
}
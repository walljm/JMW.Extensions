using Jint;
using JMW.Extensions.Enumerable;
using JMW.Extensions.String;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace JMW.Template.Tags
{
    public static class TagHelpers
    {
        public static void PrefixTags(IEnumerable<Tag> tags, string sheet_name)
        {
            foreach (var tag in tags)
            {
                if (Conditional.IsConditional(tag))
                {
                    PrefixIfColumns(tag, sheet_name);
                }
                else if (tag.TagType == TagTypes.Tag && !Output.IsOutput(tag) && !Table.IsTable(tag) && !Variable.IsVariable(tag) && !tag.Name.Contains(":"))
                {
                    PrefixTag(tag, sheet_name);
                }

                // handle children
                if (Output.IsOutput(tag) || Conditional.IsConditional(tag))
                {
                    PrefixTags(tag.Children, sheet_name);
                }
            }
        }

        public static void PrefixTag(Tag tag, string name)
        {
            if (!Output.IsOutput(tag) && !Table.IsTable(tag) && !tag.Name.Contains(":"))
            {
                tag.Name = name + ":" + tag.Name;
            }
        }

        public static void PrefixIfColumns(Tag tag, string name)
        {
            var columns = tag.Properties[Conditional.ATTR_COLUMN].Split(',').ToList();
            if (columns.Count > 1)
            {
                tag.Properties[Conditional.ATTR_COLUMN] = string.Empty;
                for (var i = 0; i < columns.Count; i++)
                {
                    var column = columns[i].Trim();
                    if (!string.IsNullOrWhiteSpace(column))
                    {
                        if (!column.Contains(":"))
                        {
                            column = name + ":" + column;
                        }
                        tag.Properties[Conditional.ATTR_COLUMN] += column + ", ";
                    }
                }
                if (tag.Properties[Conditional.ATTR_COLUMN].Contains(", "))
                    tag.Properties[Conditional.ATTR_COLUMN] = tag.Properties[Conditional.ATTR_COLUMN].ParseToLastIndexOf(", ");
            }
            // If name of column is not prefixed, conditional belongs in current sheet.
            else if (!tag.Properties[Conditional.ATTR_COLUMN].Contains(":"))
            {
                tag.Properties[Conditional.ATTR_COLUMN] = name + ":" + tag.Properties[Conditional.ATTR_COLUMN];
            }
        }

        public static void CheckAllowedAttributes(Tag curr, HashSet<string> allowed, Token token)
        {
            foreach (var p in curr.Properties)
            {
                if (!allowed.Contains(p.Key))
                    throw new ParseException("Invalid attribute found on tag. Allowed attributes: " + allowed.ToDelimitedString(", "), token);
            }
        }

        public static string RetrieveOctet(string data, string octet_string)
        {
            var val = data;
            if (!int.TryParse(octet_string, out var octet) || octet > 4 || octet < 1)
            {
                throw new Exception("The value provided for octet argument was invalid. Must be a number between 1 and 4.");
            }
            if (data.Contains("."))
            {
                var split = data.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                {
                    throw new Exception("There was a problem parsing octets from the IP: '" + data + "'. Please check formatting.");
                }
                val = split[octet - 1];
            }
            else
            {
                throw new Exception("The provided column value: '" + data + "' is not an IP.");
            }

            return val;
        }

        public static string ReplaceOctet(string data, string octet, double octet_result)
        {
            if (octet_result > 255) octet_result = 255;
            else if (octet_result < 0) octet_result = 0;

            var split = data.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            split[int.Parse(octet) - 1] = octet_result.ToString(CultureInfo.InvariantCulture);

            data = string.Empty;
            for (var i = 0; i < split.Length; i++)
            {
                data += split[i];
                if (i != split.Length - 1) data += ".";
            }
            return data;
        }

        public static bool EvaluateBooleanExpression(string expression, List<string> arguments)
        {
            var engine = setupEngine(expression, arguments);

            var result = engine.Execute(expression).GetCompletionValue().ToString();

            try
            {
                return bool.Parse(result);
            }
            catch (Exception)
            {
                throw new Exception("The expression:\n" + expression + "\ndid not evaluate to a boolean value.");
            }
        }

        public static double EvaluateArithmeticExpression(string expression, List<string> arguments)
        {
            var engine = setupEngine(expression, arguments);

            var result = engine.Execute(expression).GetCompletionValue().ToString();

            try
            {
                return double.Parse(result);
            }
            catch (Exception)
            {
                throw new Exception("The expression:\n" + expression + "\ndid not evaluate to a numeric value.");
            }
        }

        public static string EvaluateExpression(string expression, List<string> arguments)
        {
            var engine = setupEngine(expression, arguments);

            return engine.Execute(expression).GetCompletionValue().ToString();
        }

        private static Engine setupEngine(string expression, IReadOnlyList<string> arguments)
        {
            var result = new Engine();
            var variables = new List<string>();

            var matches = Regex.Matches(expression, @"(x|X)\d+");
            foreach (var match in matches)
            {
                variables.Add(match.ToString());
            }

            variables = variables.Distinct().ToList();
            if (variables.Count > arguments.Count)
            {
                throw new ArgumentException("The list of arguments provided in:\n" + arguments.ToDelimitedString(",") +
                                            "\n is greater that the number of variables listed in the expression: " + expression + "\n" +
                                            "Variables must be specified in the format: x1, x2, x3 etc. and must be equal or less than the argumenst provided.");
            }

            var i = 0;
            while (i < variables.Count && i < arguments.Count)
            {
                // try to convert to a number if possible
                if (double.TryParse(arguments[i], out var argument))
                    result.SetValue(variables[i], argument);
                else // else, use a string.
                    result.SetValue(variables[i], arguments[i]);
                i++;
            }

            return result;
        }

    }
}
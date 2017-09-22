using JMW.Extensions.String;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace JMW.Template.Tags
{
    public class Variable : TagHandlerBase
    {
        private Dictionary<string, string> _variables = new Dictionary<string, string>();

        public override string TagName { get; } = TAG;
        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_NAME, ATTR_VALUE, ATTR_OCT, ATTR_EXP, ATTR_STORE, ATTR_PRINT };
        public const string TAG = "var";
        public const string ATTR_EXP = "exp";
        public const string ATTR_OCT = "oct";
        public const string ATTR_STORE = "store";
        public const string ATTR_NAME = "name";
        public const string ATTR_VALUE = "value";
        public const string ATTR_PRINT = "print";

        public Variable()
        {
        }

        public Variable(string tag_name)
        {
            TagName = tag_name;
        }

        public override void Handler(Tag token, Interpreter interp)
        {
            //<Var name=”IP” value=”192.168.1.1” />
            if (token.Name.ToLower() == TAG)
            {
                // then its declaring the var, not calling it.
                _variables[token.Properties[ATTR_NAME].ToLower()] = token.Properties[ATTR_VALUE];
                return;
            }

            //<Var:IP oct=”2” exp=”x+5” store=”true” print=”false”/>
            var name = token.Name.ParseAfterIndexOf_PlusLength("var:");
            var value = _variables[name];
            bool store = false, print = true;

            try
            {
                if (token.Properties.ContainsKey(ATTR_STORE)) store = bool.Parse(token.Properties[ATTR_STORE]);
                if (token.Properties.ContainsKey(ATTR_PRINT)) print = bool.Parse(token.Properties[ATTR_PRINT]);
            }
            catch (Exception)
            {
                throw new Exception("One or more properties for " + token.Name +
                                    " was invalid. \"store\" and \"print\" arguments must have the value true or false.");
            }

            if (token.Properties.ContainsKey(ATTR_EXP))
            {
                var arguments = new List<string>();
                if (token.Properties.ContainsKey(ATTR_OCT))
                {
                    arguments.Add(TagHelpers.RetrieveOctet(value, token.Properties[ATTR_OCT]));
                    var octet_result = TagHelpers.EvaluateArithmeticExpression(token.Properties[ATTR_EXP], arguments);
                    value = TagHelpers.ReplaceOctet(value, token.Properties[ATTR_OCT], octet_result);
                }
                else
                {
                    arguments.Add(value);
                    value = TagHelpers.EvaluateExpression(token.Properties[ATTR_EXP], arguments).ToString(CultureInfo.InvariantCulture);
                }
            }

            if (store)
            {
                _variables[name] = value;
            }
            if (print)
            {
                interp.OutputStream.Write(value);
            }
        }

        public void AddVariable(string name, string value)
        {
            if (_variables.ContainsKey(name))
                throw new Exception("Variable: " + name + " already exists.");

            _variables.Add(name, value);
        }

        public void ReplaceVariables(Tag token)
        {
            var keys = token.Properties.Keys.ToList();
            foreach (var key in keys)
            {
                var property_value = token.Properties[key].ToLower();
                while (property_value.Contains("[" + TAG + ":") && property_value.Contains("]"))
                {
                    var full_variable = property_value.ParseAfterIndexOf("[" + TAG + ":").ParseToIndexOf("]") + "]";

                    var variable = full_variable.ParseAfterIndexOf_PlusLength("[" + TAG + ":").ParseToIndexOf("]");

                    if (_variables.ContainsKey(variable))
                    {
                        token.Properties[key] = Regex.Replace(token.Properties[key], Regex.Escape(full_variable), _variables[variable], RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        throw new Exception("Reference to variable which has not been declared: " + variable + ".");
                    }
                    property_value = token.Properties[key].ToLower();
                }
            }
        }

        public void PopulateVariables(IEnumerable<Tag> tags, Interpreter interp)
        {
            foreach (var tag in tags)
            {
                if (tag.Name == TAG)
                {
                    if (tag.Properties.ContainsKey(ATTR_NAME) && tag.Properties.ContainsKey(ATTR_VALUE))
                    {
                        if (!_variables.ContainsKey(tag.Properties[ATTR_NAME].ToLower()))
                        {
                            AddVariable(tag.Properties[ATTR_NAME], tag.Properties[ATTR_VALUE]);
                            interp.AddHandler(new Variable(TAG + ":" + tag.Properties[ATTR_NAME].ToLower()));
                        }
                        else
                        {
                            throw new Exception("Two variables with the same name (" + tag.Properties[ATTR_NAME] +
                                                ") have been used. Variable names must be unique.");
                        }
                    }
                    else
                    {
                        throw new Exception("Var tag requires both name and value arguments.");
                    }
                }
                if (tag.Children.Count > 0) PopulateVariables(tag.Children, interp);
            }
        }
    }
}
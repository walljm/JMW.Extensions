using JMW.Extensions.String;
using JMW.Template.Tags;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

// Internals are visible to the Test assemblies
[assembly: InternalsVisibleTo("JMW.Template.Tests")]

namespace JMW.Template
{
    /// <summary>
    ///   Parses template tags for a simple generic templating engine.
    /// </summary>
    public class Parser
    {
        private readonly Stack<Tag> _stack = new Stack<Tag>();
        private readonly Dictionary<string, ITagHandler> _tags = new Dictionary<string, ITagHandler>();
        private readonly HashSet<string> _columnTags = new HashSet<string>();

        public void AddColumnTag(string column)
        {
            if (!_columnTags.Contains(column))
                _columnTags.Add(column);
        }

        public void AddTag(string tag_name, ITagHandler tag)
        {
            _tags.Add(tag_name, tag);
        }

        public void AddTag(ITagHandler tag)
        {
            _tags.Add(tag.TagName, tag);
        }

        public Parser()
        {
            // add the default tags
            AddTag(new Conditional());
            AddTag(new Transform());
            AddTag(new Output());
            AddTag(new Table());
            AddTag(new Variable());
            AddTag(new Version());
            AddTag(new Timestamp());
            AddTag(new Lookup());
            AddTag(new Join());
            AddTag(new LookupRow());
        }
        
        /// <summary>
        /// Parses a template into a list of Tags to process.
        /// </summary>
        /// <param name="block">The input template</param>
        /// <returns>List of parsed Tag objects</returns>
        public List<Tag> Parse(string block)
        {
            block = handleNewLines(block);
            return Parse(new StringReader(block));
        }

        public List<Tag> Parse(TextReader reader)
        {
            var tokenizer = new Tokenizer(reader);
            var ast = new List<Tag>();
            var currParam = string.Empty;
            while (tokenizer.Next() != TokenType.ERROR)
            {
                var tag = new Tag();
                var token = tokenizer.Token;
                switch (token.Type)
                {
                    case TokenType.OPENTAG:
                        tag.TagType = TagTypes.Tag;
                        tag.Name = token.Value.ToLower().Trim();

                        if (_stack.Count > 0)
                        {
                            _stack.Peek().Children.Add(tag);
                        }
                        _stack.Push(tag);
                        break;

                    case TokenType.SINGLEClOSE:
                    case TokenType.ENDTAG:
                        var curr = _stack.Pop();

                        if (curr.Name != token.Value.ToLower().Trim() && token.Type != TokenType.SINGLEClOSE)
                        {
                            throw new ParseException("Invalid end tag.", token);
                        }

                        if (_tags.ContainsKey(curr.Name))
                        {
                            _tags[curr.Name].Validate(curr, token);
                        }

                        // handle the special case of prefixed vars.
                        else if (curr.Name.StartsWith(Variable.TAG + ":"))
                        {
                            new Variable().Validate(curr, token);
                        }
                        // handle the special case of prefixed vars.
                        else if (curr.Name == Include.TAG)
                        {
                            TagHelpers.CheckAllowedAttributes(curr, Include.ALLOWEDPROPS, token);
                        }
                        // handle the special case for columns, and join columns
                        else if (_columnTags.Contains(curr.Name) ||
                                 _columnTags.Contains(JoinColumn.LEFT_PREFIX + curr.Name) ||
                                 _columnTags.Contains(JoinColumn.RIGHT_PREFIX + curr.Name))
                        {
                            TagHelpers.CheckAllowedAttributes(curr, ColumnBase.ALLOWED, token);
                        }

                        if (_stack.Count == 0)
                        {
                            ast.Add(curr);
                        }
                        break;

                    case TokenType.PARAMNAME:
                        if (currParam.IsNotEmpty())
                        {
                            _stack.Peek().Properties[currParam] = string.Empty;
                        }
                        currParam = token.Value.ToLower();
                        break;

                    case TokenType.PARAMVALUE:
                        if (currParam.IsNotEmpty())
                        {
                            _stack.Peek().Properties[currParam] = token.Value;
                        }
                        // reset currParam since we found a value for it.
                        currParam = string.Empty;
                        break;

                    case TokenType.TEXT:
                        tag.TagType = TagTypes.Text;
                        tag.TokenText = token.Value;
                        if (_stack.Count > 0)
                        {
                            _stack.Peek().Children.Add(tag);
                        }
                        else
                        {
                            ast.Add(tag);
                        }
                        break;
                }
            }

            if (tokenizer.Token.Type == TokenType.ERROR && tokenizer.Token.Value != "EOF")
            {
                throw new ParseException(tokenizer.Token);
            }
            if (_stack.Count != 0)
            {
                throw new ParseException("Template ended prematurely. Did you forget a </" +
                    _stack.Peek().Name + ">?", tokenizer.Token);
            }
            return ast;
        }

        private static string handleNewLines(string current_template)
        {
            current_template = current_template.Replace("\r\n", string.Empty);
            current_template = current_template.Replace("\\n", "\r\n");
            return current_template;
        }
    }
}
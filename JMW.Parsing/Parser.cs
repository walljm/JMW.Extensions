using System.Collections.Generic;
using System.IO;
using JMW.Extensions.String;

namespace JMW.Parsing
{
    /// <summary>
    ///   Parses template tags for a simple generic templating engine.
    /// </summary>
    public class Parser
    {
        private readonly Stack<Tag> _stack = new Stack<Tag>();
        private readonly Dictionary<string, ITagHandler> _tags = new Dictionary<string, ITagHandler>();

        public void AddTag(string tag_name, ITagHandler tag)
        {
            _tags.Add(tag_name, tag);
        }

        public void AddTag(ITagHandler tag)
        {
            _tags.Add(tag.Name, tag);
        }

        public Parser()
        {
        }

        public Parser(Dictionary<string, ITagHandler> tags) : this()
        {
            _tags = tags;
        }

        /// <summary>
        /// Parses a template into a list of Tags to process.
        /// </summary>
        /// <param name="block">The input template</param>
        /// <returns>List of parsed Tag objects</returns>
        public List<Tag> Parse(string block)
        {
            return Parse(new StringReader(block));
        }

        public List<Tag> Parse(TextReader reader)
        {
            var tokenizer = new Tokenizer(reader);
            var ast = new List<Tag>();

            while (tokenizer.Next() != TokenType.Error)
            {
                var tag = new Tag();
                var token = tokenizer.Token;
                switch (token.Type)
                {
                    case TokenType.ObjectStart:
                        tag.TagType = TagTypes.Object;
                        tag.Name = token.Value.ToLower().Trim();
                        _stack.Push(tag);
                        break;

                    case TokenType.ArrayStart:
                        tag.TagType = TagTypes.Array;
                        tag.Name = token.Value.ToLower().Trim();
                        tag.Value = new Stack<Tag>();
                        _stack.Push(tag);

                        break;

                    case TokenType.ArrayStop:
                        {
                            var curr = _stack.Pop();
                            if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Property)
                            {
                                var t = _stack.Pop();
                                _stack.Peek().Properties[t.Name] = curr;
                                break;
                            }
                            _stack.Peek().Properties[curr.Name] = curr;
                            break;
                        }
                    case TokenType.ObjectStop:
                        {
                            
                            var curr = _stack.Pop();
                            if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Array)
                            {
                                var o = (Stack<Tag>)_stack.Peek().Value;
                                o.Push(curr);
                                break;
                            }
                            if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Property)
                            {
                                var t = _stack.Pop();
                                _stack.Peek().Properties[t.Name] = curr;
                                break;
                            }

                            if (_stack.Count == 0)
                            {
                                ast.Add(curr);
                            }
                            break;
                        }
                    case TokenType.PropertyName:
                        tag.Name = token.Value.ToLower().Trim();
                        tag.TagType = TagTypes.Property;
                        _stack.Push(tag);
                        
                        break;

                    case TokenType.Value:
                        if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Property)
                        {
                            var o = _stack;
                            if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Array)
                                o = (Stack<Tag>)_stack.Peek().Value;

                            var t = o.Pop();
                            t.Value = token.Value;
                            t.TagType = TagTypes.Property;
                            _stack.Peek().Properties[t.Name] = t;
                        }
                        if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Array)
                        {
                            var o = (Stack<Tag>)_stack.Peek().Value;
                            tag.TagType = TagTypes.Word;
                            tag.Name = string.Empty;
                            o.Push(tag);
                        }
                        break;
                }
            }

            if (tokenizer.Token.Type == TokenType.Error && tokenizer.Token.Value != "EOF")
            {
                throw new ParseException(tokenizer.Token);
            }
            if (_stack.Count != 0)
            {
                throw new ParseException("Parsing code ended prematurely. Did you forget a closing }?", tokenizer.Token);
            }
            return ast;
        }
    }
}
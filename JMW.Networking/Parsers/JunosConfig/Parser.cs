using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JMW.Networking.Parsers.JunosConfig;

/// <summary>
///   Parses template tags for a simple generic templating engine.
/// </summary>
public class Parser
{
    private readonly Stack<Tag> _stack = new Stack<Tag>();

    public Parser()
    {
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
        Tag last = null;
        while (tokenizer.Next() != TokenType.Error)
        {
            var tag = new Tag();
            var token = tokenizer.Token;
            switch (token.Type)
            {
                case TokenType.ObjectStart:
                    var o_names = collapseWords();
                    if (o_names.Count > 0)
                    {
                        tag.Name = o_names.First().Value.Trim();
                        tag.Value = (string.Join(" ", o_names.Skip(1).Select(o => o.Value).ToArray()) + " " + token.Value.Trim()).Trim();
                    }
                    else
                        tag.Name = token.Value.Trim();

                    tag.TagType = TagTypes.Object;
                    tag.Children = new Stack<Tag>();
                    _stack.Push(tag);
                    break;

                case TokenType.ArrayStart:
                    if (_stack.Count > 0)
                        _stack.Pop(); // you don't need the current word.
                    tag.TagType = TagTypes.Array;
                    tag.Name = token.Value.ToLower().Trim();
                    tag.Children = new Stack<Tag>();
                    _stack.Push(tag);
                    break;

                case TokenType.ArrayStop:
                    {
                        var curr = _stack.Pop();
                        if (_stack.Count == 0)
                        {
                            ast.Add(curr);
                        }
                        else
                        {
                            _stack.Peek().Children.Push(curr);
                        }
                        break;
                    }
                case TokenType.ObjectStop:
                    {
                        var curr = _stack.Pop();
                        last = curr;
                        if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Array)
                        {
                            _stack.Peek().Children.Push(curr);
                            break;
                        }
                        if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Object)
                        {
                            _stack.Peek().Children.Push(curr);
                            break;
                        }

                        if (_stack.Count == 0)
                        {
                            ast.Add(curr);
                        }
                        break;
                    }
                case TokenType.LineStop:
                    {
                        if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Array)
                        {
                            tag.TagType = TagTypes.Word;
                            tag.Name = string.Empty;
                            tag.Value = token.Value;
                            _stack.Peek().Children.Push(tag);
                        }
                        else if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Object)
                        {
                            if (token.Value.Length == 0)
                                break;

                            tag.TagType = TagTypes.Word;
                            tag.Name = string.Empty;
                            tag.Value = token.Value;
                            _stack.Peek().Children.Push(tag);
                        }
                        else if (_stack.Count > 0)
                        {
                            var words = collapseWords();
                            tag.TagType = TagTypes.Property;

                            if (words.Count > 2)
                            {
                                tag.Name = words.Last().Value;

                                tag.Value = string.Join(" ", words.Take(words.Count - 1).Select(o => o.Value).ToArray());
                            }
                            else if (words.Count == 2)
                            {
                                tag.Name = words[1].Value;
                                tag.Value = words[0].Value;
                            }
                            else if (words.Count == 1)
                            {
                                tag.Name = words[0].Value;
                            }

                            if (_stack.Count > 0)
                            {
                                _stack.Peek().Children.Push(tag);
                            }
                            else
                                ast.Add(tag);
                        }
                        break;
                    }
                case TokenType.Word:
                    {
                        tag.TagType = TagTypes.Word;
                        tag.Value = token.Value;
                        if (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Array)
                        {
                            _stack.Peek().Children.Push(tag);
                        }
                        else
                            _stack.Push(tag);
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

    private List<Tag> collapseWords()
    {
        var words = new List<Tag>();
        while (_stack.Count > 0 && _stack.Peek().TagType == TagTypes.Word)
            words.Add(_stack.Pop());
        return words;
    }
}
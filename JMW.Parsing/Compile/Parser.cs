using System.Collections.Generic;
using System.IO;

namespace JMW.Parsing.Compile;

/// <summary>
///   Parses template tags for a simple generic templating engine.
/// </summary>
public class Parser
{
    private readonly Stack<Tag> stack = new();

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
        return this.Parse(new StringReader(block));
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
                    tag.TagType = TagTypes.Object;
                    tag.Name = token.Value.ToLower().Trim();
                    this.stack.Push(tag);
                    break;

                case TokenType.ArrayStart:
                    tag.TagType = TagTypes.Array;
                    tag.Name = token.Value.ToLower().Trim();
                    tag.Value = new Stack<Tag>();
                    this.stack.Push(tag);

                    break;

                case TokenType.ArrayStop:
                    {
                        var curr = this.stack.Pop();
                        last = curr;
                        if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Property)
                        {
                            var t = this.stack.Pop();
                            this.stack.Peek().Properties[t.Name] = curr;
                            break;
                        }
                        this.stack.Peek().Properties[curr.Name] = curr;
                        break;
                    }
                case TokenType.ObjectStop:
                    {
                        var curr = this.stack.Pop();
                        last = curr;
                        if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Array)
                        {
                            var o = (Stack<Tag>)this.stack.Peek().Value;
                            o.Push(curr);
                            break;
                        }
                        if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Property)
                        {
                            var t = this.stack.Pop();
                            this.stack.Peek().Properties[t.Name] = curr;
                            break;
                        }

                        if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Object)
                        {
                            // its an object like an Or or and And that has a list of expressions.
                            var o = this.stack.Peek();
                            if (o.Value.GetType() != typeof(Stack<Tag>))
                            {
                                o.Value = new Stack<Tag>();
                            }

                            ((Stack<Tag>)o.Value).Push(curr);
                            break;
                        }

                        if (this.stack.Count == 0)
                        {
                            ast.Add(curr);
                        }
                        break;
                    }
                case TokenType.PropertyName:
                    tag.Name = token.Value.ToLower().Trim();
                    tag.TagType = TagTypes.Property;
                    this.stack.Push(tag);

                    break;

                case TokenType.Options:
                    tag.Name = "m";
                    tag.Value = token.Value;
                    tag.TagType = TagTypes.Property;
                    last?.Properties.Add(tag.Name, tag);
                    break;

                case TokenType.Value:
                    if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Property)
                    {
                        var o = this.stack;
                        if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Array)
                            o = (Stack<Tag>)this.stack.Peek().Value;

                        var t = o.Pop();
                        t.Value = token.Value;
                        t.TagType = TagTypes.Property;
                        this.stack.Peek().Properties[t.Name] = t;
                    }
                    if (this.stack.Count > 0 && this.stack.Peek().TagType == TagTypes.Array)
                    {
                        var o = (Stack<Tag>)this.stack.Peek().Value;
                        tag.TagType = TagTypes.Word;
                        tag.Name = string.Empty;
                        tag.Value = token.Value;
                        o.Push(tag);
                    }
                    break;
            }
        }

        if (tokenizer.Token.Type == TokenType.Error && tokenizer.Token.Value != "EOF")
        {
            throw new ParseException(tokenizer.Token);
        }

        if (this.stack.Count != 0)
        {
            throw new ParseException("Parsing code ended prematurely. Did you forget a closing }?", tokenizer.Token);
        }

        return ast;
    }
}
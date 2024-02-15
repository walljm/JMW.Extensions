using JMW.Collections;
using JMW.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JMW.Networking.Parsers.JunosConfig;

internal class Tokenizer
{
    private static readonly HashSet<char> unacceptableWordChars = new("{}[]#;".ToCharArray());
    private readonly string objectStart = @"{";
    private readonly string objectStop = @"}";
    private readonly string arrayStart = @"[";
    private readonly string arrayStop = @"]";
    private readonly string commentBlockStart = @"/*";
    private readonly string commentStart = @"#";
    private readonly string lineStop = @";";

    private readonly LineTrackingReader reader;

    internal Token Token { get; } = new();

    internal static readonly string INVALID_UNICODE_ERROR = "Invalid Character Encoding for parsing instruction input";

    private bool inWord = false;
    private bool eof = false;

    internal int Line => reader.Line;
    internal int Column => reader.Column;

    internal Tokenizer(string template)
        : this(new StringReader(template))
    {
    }

    internal Tokenizer(TextReader reader)
    {
        this.reader = new(reader);
    }

    internal TokenType Next()
    {
        try
        {
            if (eof)
            {
                return SetupErrorToken("EOF");
            }
            Token.Line = Line;
            Token.Column = Column;

            if (inWord)
            {
                inWord = false;

                // you have a word, is it a property or an object?
                if (MaybeReadObjectStart())
                {
                    return ConsumeObjectStart();
                }

                // ignore empty words.
                if (Token.Value.Length > 0)
                {
                    Token.Type = TokenType.Word;
                    return Token.Type;
                }
            }

            if (MaybeReadArrayStart())
            {
                return ConsumeArrayStart();
            }

            if (MaybeReadCommentBlockStart())
            {
                return ConsumeCommentBlockStart();
            }

            if (MaybeReadCommentStart())
            {
                return ConsumeComment();
            }

            if (MaybeReadObjectStop())
            {
                return ConsumeObjectStop();
            }

            if (MaybeReadArrayStop())
            {
                return ConsumeArrayStop();
            }
            if (MaybeReadLineStop())
            {
                return ConsumeLineStop();
            }
            return MaybeReadWord() ? ConsumeWord() : ConsumeWhitespace();
        }
        catch (Exception ex)
        {
            return SetupErrorToken(ex.Message);
        }
    }

    private TokenType SetupErrorToken(string msg)
    {
        return SetupErrorToken(msg, Line, Column);
    }

    private TokenType SetupErrorToken(string msg, int line, int column)
    {
        Token.Type = TokenType.Error;
        Token.Value = msg;
        Token.Line = line;
        Token.Column = column;
        return Token.Type;
    }

    private TokenType ConsumeUntil(int cc, TokenType type)
    {
        Token.Type = type;
        var c = reader.Read();
        var value = new List<char>();
        while (c != cc && c != 0)
        {
            value.Add((char)c);
            c = reader.Read();
            // This is the unicode invalid character. If we encounter this it means we parsed the
            // template with an invalid encoding. Or the template was stored with an invalid
            // encoding.
            if (c == '\uffff')
            {
                return SetupErrorToken(INVALID_UNICODE_ERROR);
            }
        }
        if (c == -1)
        {
            eof = true;
        }
        else if (c != cc)
        {
            reader.PushBack((char)c);
        }
        Token.Value = new string(value.ToArray());
        return type;
    }

    private TokenType ConsumeComment()
    {
        var c = reader.Read();
        if (c == -1)
        {
            eof = true;
            return SetupErrorToken("EOF");
        }
        reader.PushBack([(char)c]);

        ConsumeUntil('\n', TokenType.Comment);
        Token.Value = Token.Value.TrimEnd('\r', '\n');
        return Token.Type;
    }

    private TokenType ConsumeObjectStart()
    {
        Token.Type = TokenType.ObjectStart;
        return Token.Type;
    }

    private TokenType ConsumeObjectStop()
    {
        Token.Value = string.Empty;
        Token.Type = TokenType.ObjectStop;
        return Token.Type;
    }

    private TokenType ConsumeArrayStart()
    {
        Token.Type = TokenType.ArrayStart;
        return Token.Type;
    }

    private TokenType ConsumeCommentBlockStart()
    {
        var text = new RingBuffer<char>(2);

        var c = reader.Read();
        text.Add((char)c);

        while (!(text[0] == '*' && text[1] == '/')) // ignore the ';', its optional.
        {
            c = reader.Read();
            // This is the unicode invalid character. If we encounter this it means we parsed the
            // template with an invalid encoding. Or the template was stored with an invalid
            // encoding.
            if (c == '\uffff')
            {
                return SetupErrorToken(INVALID_UNICODE_ERROR);
            }
            text.Add((char)c);
        }

        if (c == -1)
        {
            eof = true;
        }

        return Next();
    }

    private TokenType ConsumeArrayStop()
    {
        Token.Value = string.Empty;
        Token.Type = TokenType.ArrayStop;
        return Token.Type;
    }

    private TokenType ConsumeLineStop()
    {
        Token.Value = string.Empty;
        Token.Type = TokenType.LineStop;
        return Token.Type;
    }

    private TokenType ConsumeWord()
    {
        var identifier = new List<char>();
        var c = reader.Read();
        if ((char)c == '"')
        {
            inWord = true;
            reader.PushBack((char)c);
            return ConsumeQuotedString();
        }

        while (!char.IsWhiteSpace((char)c) && !unacceptableWordChars.Contains((char)c))
        {
            identifier.Add((char)c);
            c = reader.Read();
            // This is the unicode invalid character. If we encounter this it means we parsed the
            // template with an invalid encoding. Or the template was stored with an invalid
            // encoding.
            if (c == '\uffff')
            {
                return SetupErrorToken(INVALID_UNICODE_ERROR);
            }
        }
        if (c == -1)
        {
            eof = true;
        }
        else
        {
            reader.PushBack((char)c);
        }

        inWord = true;
        Token.Type = TokenType.Word;
        Token.Value = new string(identifier.ToArray());
        return ConsumeWhitespace();
    }

    private TokenType ConsumeQuotedString()
    {
        var text = new StringBuilder();

        var c = reader.Read();
        if (c == 34)
            c = reader.Read();
        else
        {
            return Next();
        }

        text.Append((char)c);

        while (c != 34) // ignore the ';', its optional.
        {
            c = reader.Read();
            // This is the unicode invalid character. If we encounter this it means we parsed the
            // template with an invalid encoding. Or the template was stored with an invalid
            // encoding.
            if (c == '\uffff')
            {
                return SetupErrorToken(INVALID_UNICODE_ERROR);
            }
            text.Append((char)c);
        }

        if (c == -1)
        {
            eof = true;
        }
        Token.Value = text.ToString().Trim('"');
        Token.Type = TokenType.Word;
        return Next();
    }

    private TokenType ConsumeWhitespace()
    {
        var c = reader.Read();
        while (c != -1 && char.IsWhiteSpace((char)c)) // ignore the ';', its optional.
        {
            c = reader.Read();
            // This is the unicode invalid character. If we encounter this it means we parsed the
            // template with an invalid encoding. Or the template was stored with an invalid
            // encoding.
            if (c == '\uffff')
            {
                return SetupErrorToken(INVALID_UNICODE_ERROR);
            }
        }
        if (c == -1)
        {
            eof = true;
        }
        else
        {
            reader.PushBack((char)c);
        }
        return Next();
    }

    private bool MaybeReadWord()
    {
        ReadText(" ", out var buf, out var n);
        reader.PushBack(buf.Take(n).ToArray());

        var c = buf.First();
        return !unacceptableWordChars.Contains(c);
    }

    private bool MaybeReadCommentStart()
    {
        return MaybeReadText(commentStart);
    }

    private bool MaybeReadObjectStart()
    {
        return MaybeReadText(objectStart);
    }

    private bool MaybeReadObjectStop()
    {
        return MaybeReadText(objectStop);
    }

    private bool MaybeReadArrayStart()
    {
        return MaybeReadText(arrayStart);
    }

    private bool MaybeReadCommentBlockStart()
    {
        return MaybeReadText(commentBlockStart);
    }

    private bool MaybeReadArrayStop()
    {
        return MaybeReadText(arrayStop);
    }

    private bool MaybeReadLineStop()
    {
        return MaybeReadText(lineStop);
    }

    private bool MaybeReadBeginPropertyValue()
    {
        var c = reader.Read();
        reader.PushBack((char)c);

        return c == '"' || c == '\'';
    }

    private bool MaybeReadText(string text)
    {
        ReadText(text, out var buf, out var n);
        var ok = n == text.Length && new string(buf) == text;
        if (!ok)
        {
            reader.PushBack(buf.Take(n).ToArray());
        }
        return ok;
    }

    private void ReadText(string text, out char[] buf, out int n)
    {
        buf = new char[text.Length];
        n = reader.Read(buf);

        if (n == 0)
        {
            eof = true;
        }
    }
}
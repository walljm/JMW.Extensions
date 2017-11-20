using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JMW.IO;

[assembly: InternalsVisibleTo("JMW.Template.Tests")]

namespace JMW.Template
{
    public class Tokenizer
    {
        internal LineTrackingReader _reader;
        public Token Token
        {
            get;
        }

        public static readonly string INVALID_UNICODE_ERROR = "Invalid Character Encoding for template input";

        private readonly string _begin = @"<";
        private readonly string _end = @"</";
        private readonly string _close = @">";
        private readonly string _singleClose = @"/>";
        private readonly string _slash = @"/";

        internal bool _inTag = false;
        internal bool _inParam = false;
        internal bool _eof = false;

        internal int Line
        {
            get { return _reader.Line; }
        }

        internal int Column
        {
            get { return _reader.Column; }
        }

        public Tokenizer(string template)
            : this(new StringReader(template))
        {
        }

        public Tokenizer(TextReader reader)
        {
            _reader = new LineTrackingReader(reader);
            Token = new Token();
        }

        public TokenType Next()
        {
            NEXT_START:
            try
            {
                if (_eof)
                {
                    return setupErrorToken("EOF");
                }
                Token.Line = Line;
                Token.Column = Column;
                if (maybeReadEnd())
                {
                    return consumeTagName(TokenType.ENDTAG);
                }
                else if (maybeReadBegin())
                {
                    return consumeTagName(TokenType.OPENTAG);
                }
                else if (maybeReadClose()) // We just eat close sigils
                {
                    _inTag = false;
                    // yes yes it's a goto. This is a state machine deal with it.
                    goto NEXT_START;
                }
                else if (maybeReadSingleClose())
                {
                    _inTag = false;
                    Token.Type = TokenType.SINGLEClOSE;
                    Token.Value = "";
                    return Token.Type;
                }
                else if (_inTag) // if we are in a tag then we expect to start reading parameters.
                {
                    if (maybeReadText("="))
                    {
                        if (!_inParam)
                        {
                            return setupErrorToken("Unexpected '='. Did you forget a param name?", Line, Column - 1);
                        }
                        else
                        {
                            return consumeParamValue();
                        }
                    }
                    else
                    {
                        return consumeParamName();
                    }
                }
                else // Anything else is just text.
                {
                    return consumeTextOrTag();
                }
            }
            catch (Exception ex)
            {
                return setupErrorToken(ex.Message);
            }
        }

        private TokenType setupErrorToken(string msg)
        {
            return setupErrorToken(msg, Line, Column);
        }

        private TokenType setupErrorToken(string msg, int line, int column)
        {
            Token.Type = TokenType.ERROR;
            Token.Value = msg;
            Token.Line = line;
            Token.Column = column;
            return Token.Type;
        }

        internal TokenType consumeTextOrTag()
        {
            Token.Type = TokenType.TEXT;
            var consumed = false;
            var tb = new StringBuilder();
            var c = _reader.Read();
            while (c != -1 && c != '<')
            {
                // This is the unicode invalid character. If we encounter this it means we parsed the
                // template with an invalid encoding. Or the template was stored with an invalid
                // encoding.
                if (c == '\uffff')
                {
                    return setupErrorToken(INVALID_UNICODE_ERROR);
                }
                consumed = true;
                tb.Append((char)c);
                c = _reader.Read();
            }
            if (c != -1) // NOTE(jeremy): -1 when cast to a char is an invalid unicode char. Don't push it back!
            {
                _reader.PushBack((char)c);
            }
            if (consumed)
            {
                Token.Value = tb.ToString();
                return Token.Type;
            }
            else // this should only trigger if we had an immediate tag or eof.
            {
                return Next();
            }
        }

        internal TokenType consumeParamValue()
        {
            var c = _reader.Read();
            if (c == -1)
            {
                _eof = true;
                return setupErrorToken("EOF");
            }
            if (c == '"' || c == '\'')
            {
                return consumeUntil(c, TokenType.PARAMVALUE);
            }
            else
            {
                _reader.PushBack((char)c);
                return consumeIdentifier(TokenType.PARAMVALUE);
            }
        }

        internal TokenType consumeUntil(int cc, TokenType type)
        {
            Token.Type = type;
            var c = _reader.Read();
            var value = new List<char>();
            while (c != cc && c != 0)
            {
                value.Add((char)c);
                c = _reader.Read();
            }
            if (c == -1)
            {
                _eof = true;
            }
            else if (c != cc)
            {
                _reader.PushBack((char)c);
            }
            Token.Value = new string(value.ToArray());
            return type;
        }

        internal TokenType consumeParamName()
        {
            _inParam = true;
            return consumeIdentifier(TokenType.PARAMNAME);
        }

        internal TokenType consumeTagName(TokenType type)
        {
            if (_inTag)
            {
                return setupErrorToken("Invalid tag. Did you forget to add a '>'?", Line, Column - 2);
            }
            _inTag = true;
            return consumeIdentifier(type);
        }

        internal TokenType consumeIdentifier(TokenType type)
        {
            var identifier = new List<char>();
            var c = _reader.Read();
            while (c != -1 && c != '/' && c != '>' && c != '<' && c != '=' && !Char.IsWhiteSpace((char)c))
            {
                identifier.Add((char)c);
                c = _reader.Read();
            }
            if (c == -1)
            {
                _eof = true;
            }
            else
            {
                _reader.PushBack((char)c);
            }
            Token.Type = type;
            Token.Value = new string(identifier.ToArray());
            consumeWhitespace();
            return type;
        }

        internal void consumeWhitespace()
        {
            var c = _reader.Read();
            while (c != -1 && Char.IsWhiteSpace((char)c))
            {
                c = _reader.Read();
            }
            if (c == -1)
            {
                _eof = true;
            }
            else
            {
                _reader.PushBack((char)c);
            }
        }

        internal bool maybeReadBegin()
        {
            return maybeReadText(_begin);
        }

        internal bool maybeReadEnd()
        {
            return maybeReadText(_end);
        }

        internal bool maybeReadClose()
        {
            return maybeReadText(_close);
        }

        internal bool maybeReadSingleClose()
        {
            var ok = maybeReadText(_singleClose);

            if (!ok)
            {
                var isslash = maybeReadText(_slash);
                if (isslash && _inTag)
                    throw new Exception("Single tag closing forward slash detected without closing angle bracket.");
                else if (isslash)
                {
                    // put the text back.
                    _reader.PushBack(_slash.ToCharArray().First());
                }
            }

            return ok;
        }

        internal bool maybeReadText(string text)
        {
            char[] buf;
            int n;
            readText(text, out buf, out n);
            var ok = (n == text.Length && new String(buf) == text);
            if (!ok)
            {
                _reader.PushBack(buf.Take(n).ToArray());
            }
            return ok;
        }

        private void readText(string text, out char[] buf, out int n)
        {
            buf = new char[text.Length];
            n = _reader.Read(buf);
            if (n == 0)
            {
                _eof = true;
            }
        }
    }
}
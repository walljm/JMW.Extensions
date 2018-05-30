using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JMW.IO;

namespace JMW.Parsing.Compile
{
    public class Tokenizer
    {
        private readonly string _objectStart = @"{";
        private readonly string _objectStop = @"}";
        private readonly string _arrayStart = @"[";
        private readonly string _arrayStop = @"]";
        private readonly string _propertyStart = @":";
        private readonly string _commentStart = @"#";

        internal LineTrackingReader _reader;
        public Token Token
        {
            get;
        }

        public static readonly string INVALID_UNICODE_ERROR = "Invalid Character Encoding for parsing instruction input";

        internal bool _inWord = false;
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
            try
            {
                if (_eof)
                {
                    return setupErrorToken("EOF");
                }
                Token.Line = Line;
                Token.Column = Column;

                if (_inWord)
                {
                    _inWord = false;

                    // you have a word, is it a property or an object?
                    if (maybeReadObjectStart())
                    {
                        return consumeObjectStart();
                    }

                    if (maybeReadPropertyStart())
                    {
                        return consumePropertyName();
                    }

                    // if its not a prop and its not an object, then its a set of options.
                    Token.Type = TokenType.Options;
                    return Token.Type;
                }

                if (maybeReadArrayStart())
                {
                    return consumeArrayStart();
                }

                if (maybeReadCommentStart())
                {
                    return consumeComment();
                }

                if (maybeReadObjectStop())
                {
                    return consumeObjectStop();
                }

                if (maybeReadArrayStop())
                {
                    return consumeArrayStop();
                }

                if (maybeReadBeginPropertyValue())
                {
                    return consumePropertyValue();
                }

                if (maybeReadWord())
                {
                    return consumeWord();
                }

                return consumeWhitespace();
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
            Token.Type = TokenType.Error;
            Token.Value = msg;
            Token.Line = line;
            Token.Column = column;
            return Token.Type;
        }

        internal TokenType consumeUntil(int cc, TokenType type)
        {
            Token.Type = type;
            var c = _reader.Read();
            var value = new List<char>();
            while (c != cc && c != 0 && c != -1)
            {
                value.Add((char)c);
                c = _reader.Read();
                // This is the unicode invalid character. If we encounter this it means we parsed the
                // template with an invalid encoding. Or the template was stored with an invalid
                // encoding.
                if (c == '\uffff')
                {
                    return setupErrorToken(INVALID_UNICODE_ERROR);
                }
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

        internal TokenType consumeComment()
        {
            var c = _reader.Read();
            if (c == -1)
            {
                _eof = true;
                return setupErrorToken("EOF");
            }
            _reader.PushBack(new[] { (char)c });

            consumeUntil('\n', TokenType.Comment);
            Token.Value = Token.Value.TrimEnd('\r', '\n');
            return Token.Type;
        }

        internal TokenType consumePropertyValue()
        {
            var c = _reader.Read();
            if (c == -1)
            {
                _eof = true;
                return setupErrorToken("EOF");
            }
            if (c == '"' || c == '\'')
            {
                return consumeUntil(c, TokenType.Value);
            }
            _reader.PushBack((char)c);
            return Token.Type;
        }

        internal TokenType consumePropertyName()
        {
            Token.Type = TokenType.PropertyName;
            return Token.Type;
        }

        internal TokenType consumeObjectStart()
        {
            Token.Type = TokenType.ObjectStart;
            return Token.Type;
        }

        internal TokenType consumeObjectStop()
        {
            Token.Value = string.Empty;
            Token.Type = TokenType.ObjectStop;
            return Token.Type;
        }

        internal TokenType consumeArrayStart()
        {
            Token.Type = TokenType.ArrayStart;
            return Token.Type;
        }

        internal TokenType consumeArrayStop()
        {
            Token.Value = string.Empty;
            Token.Type = TokenType.ArrayStop;
            return Token.Type;
        }

        internal TokenType consumeWord()
        {
            var acceptable = "abcdefghijklmnopqrstuvwxyz_-".ToCharArray();

            var identifier = new List<char>();
            var c = _reader.Read();
            while (!Char.IsWhiteSpace((char)c) && acceptable.Contains((char)c))
            {
                identifier.Add((char)c);
                c = _reader.Read();
                // This is the unicode invalid character. If we encounter this it means we parsed the
                // template with an invalid encoding. Or the template was stored with an invalid
                // encoding.
                if (c == '\uffff')
                {
                    return setupErrorToken(INVALID_UNICODE_ERROR);
                }
            }
            if (c == -1)
            {
                _eof = true;
            }
            else
            {
                _reader.PushBack((char)c);
            }
            _inWord = true;
            Token.Type = TokenType.Word;
            Token.Value = new string(identifier.ToArray());
            return consumeWhitespace();
        }

        internal TokenType consumeWhitespace()
        {
            var c = _reader.Read();
            while (c != -1 && (Char.IsWhiteSpace((char)c) || c == ';') || c == ',') // ignore the ';', its optional.
            {
                c = _reader.Read();
                // This is the unicode invalid character. If we encounter this it means we parsed the
                // template with an invalid encoding. Or the template was stored with an invalid
                // encoding.
                if (c == '\uffff')
                {
                    return setupErrorToken(INVALID_UNICODE_ERROR);
                }
            }
            if (c == -1)
            {
                _eof = true;
            }
            else
            {
                _reader.PushBack((char)c);
            }
            return Next();
        }

        internal bool maybeReadWord()
        {
            readText(" ", out var buf, out var n);
            _reader.PushBack(buf.Take(n).ToArray());

            var c = buf.First();
            var acceptable = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            if (acceptable.Contains(c))
                return true;

            return false;
        }

        internal bool maybeReadCommentStart()
        {
            return maybeReadText(_commentStart);
        }

        internal bool maybeReadObjectStart()
        {
            return maybeReadText(_objectStart);
        }

        internal bool maybeReadObjectStop()
        {
            return maybeReadText(_objectStop);
        }

        internal bool maybeReadArrayStart()
        {
            return maybeReadText(_arrayStart);
        }

        internal bool maybeReadArrayStop()
        {
            return maybeReadText(_arrayStop);
        }

        internal bool maybeReadPropertyStart()
        {
            return maybeReadText(_propertyStart);
        }

        internal bool maybeReadBeginPropertyValue()
        {
            var c = _reader.Read();
            _reader.PushBack((char)c);

            if (c == '"' || c == '\'')
                return true;

            return false;
        }

        internal bool maybeReadText(string text)
        {
            readText(text, out var buf, out var n);
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
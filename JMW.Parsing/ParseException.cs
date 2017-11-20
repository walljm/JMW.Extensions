using System;

namespace JMW.Parsing
{
    [Serializable]
    public class ParseException : Exception
    {
        public string Location { get; } = "";
        public string ErrorText { get; } = "";

        public ParseException(string message)
            : base(message)
        {
        }

        public ParseException(string message, Token token)
            : base(message + " " + token.LocationToString())
        {
            Location = token.LocationToString();
            ErrorText = message;
        }

        public ParseException(Token token)
            : base(token.ToString())
        {
            Location = token.LocationToString();
            ErrorText = token.Type + ": " + token.Value;
        }
    }
}
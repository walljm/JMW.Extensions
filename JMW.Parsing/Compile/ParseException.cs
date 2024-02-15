using System;

namespace JMW.Parsing.Compile;

[Serializable]
public class ParseException : Exception
{
    public string Location { get; } = string.Empty;
    public string ErrorText { get; } = string.Empty;

    public ParseException(string message)
        : base(message)
    {
    }

    public ParseException(string message, Token token)
        : base(message + " " + token.LocationToString())
    {
        this.Location = token.LocationToString();
        this.ErrorText = message;
    }

    public ParseException(Token token)
        : base(token.ToString())
    {
        this.Location = token.LocationToString();
        this.ErrorText = token.Type + ": " + token.Value;
    }
}
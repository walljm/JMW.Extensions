using System;

namespace JMW.Networking.Parsers.JunosConfig;

[Serializable]
public class ParseException : Exception
{
    public string Location { get; } = string.Empty;
    public string ErrorText { get; } = string.Empty;

    public ParseException(string message)
        : base(message)
    {
    }

    internal ParseException(string message, Token token)
    : base(message + " " + token.LocationToString())
    {
        Location = token.LocationToString();
        ErrorText = message;
    }

    internal ParseException(Token token)
        : base(token.ToString())
    {
        Location = token.LocationToString();
        ErrorText = token.Type + ": " + token.Value;
    }
}
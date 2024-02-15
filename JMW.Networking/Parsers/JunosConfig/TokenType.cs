namespace JMW.Networking.Parsers.JunosConfig;

internal enum TokenType
{
    ObjectStop,
    ObjectStart,
    ArrayStop,
    ArrayStart,
    LineStop,
    Word,
    Comment,
    Error
}
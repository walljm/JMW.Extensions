namespace JMW.Parsing.Compile;

public enum TokenType
{
    PropertyName,
    ObjectStop,
    ObjectStart,
    ArrayStop,
    ArrayStart,
    Word,
    Value,
    Comment,
    Options,
    Error
}
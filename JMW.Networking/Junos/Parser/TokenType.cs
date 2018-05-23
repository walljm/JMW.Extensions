namespace JMW.Networking.Junos.Parser
{
    public enum TokenType
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
}
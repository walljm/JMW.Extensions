namespace JMW.Parsing.Compile;

public class Token
{
    public string Value;
    public TokenType Type;
    public int Line;
    public int Column;

    public string LocationToString()
    {
        return $"Line: {this.Line + 1} Column: {this.Column + 1}";
    }

    public override string ToString()
    {
        return $"{this.Value} {this.LocationToString()}";
    }
}
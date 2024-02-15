using System.Collections.Generic;
using System.IO;

namespace JMW.Networking.Parsers;

public static class NetStat
{
    #region Public Parse/Output

    public static void Parse(TextReader output, ParsingOptions options)
    {
        // grab tables
    }

    #endregion Public Parse/Output
}

public record ColumnPosition
{
    public string? Name { get; set; }
    public string? Key { get; set; }
    public int Start { get; set; } = -1;
    public int Length { get; set; } = -1;
}

public class ColumnPositionCollection
{
    public ColumnPositionCollection(Dictionary<string, ColumnPosition> positions)
    {
        Positions = positions;
        int start = -1;
        foreach (var pos in positions)
        {
            if (pos.Value.Start > start) start = pos.Value.Start;
        }
        LastStartPos = start;
    }

    public Dictionary<string, ColumnPosition> Positions { get; set; } = new();

    public int LastStartPos { get; } = -1;
}

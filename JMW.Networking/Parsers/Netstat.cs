using JMW.Parsing.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JMW.Networking.Parsers;

public static class NetStat
{
    public static IEnumerable<IDictionary<string, string>> Parse(TextReader reader)
    {
        // grab tables
        var tables = ExtractTables(reader);

        // parse tables
        foreach (var table in tables)
        {
            var lines = table.Split('\n');
            var positions = Table.DeriveColumnPositions(lines.Skip(1).ToArray());

            foreach (var o in lines.Skip(2))
            {
                if (string.IsNullOrWhiteSpace(o))
                {
                    continue;
                }

                var row = new Dictionary<string, string>();
                foreach (var position in positions.Positions)
                {
                    var val = position.Value.Length == -1
                        ? o.Substring(position.Value.Start)
                        : o.Substring(position.Value.Start, position.Value.Length);

                    row.Add(position.Key, val.Trim());
                }
                yield return row;
            }
        }
    }

    private static readonly StringComparison ignoreCase = StringComparison.OrdinalIgnoreCase;

    private static IEnumerable<string> ExtractTables(TextReader output)
    {
        var sb = new StringBuilder();
        string? line;
        while ((line = output.ReadLine()) is not null)
        {
            if (
                   string.IsNullOrWhiteSpace(line)
                || line.StartsWith("netstat", ignoreCase)
                || line.StartsWith("routing", ignoreCase)
            )
            {
                continue;
            }

            if (line.StartsWith("internet", ignoreCase) && sb.Length > 0)
            {
                // new block
                yield return sb.ToString();
                sb.Clear();
            }

            sb.AppendLine(line);
        }

        yield return sb.ToString();
    }
}

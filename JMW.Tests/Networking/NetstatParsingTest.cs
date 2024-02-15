using JMW.Networking.Parsers;
using JMW.Tests;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace JMW.Parsing.Tests;

[TestFixture]
public class NetstatParsingTest : TestBase
{
    [Test]
    public void ParseTest()
    {
        // var dir = Path.Combine(ExecutionPath, "Networking", "macnetstat.txt");
        // using var st = new FileStream(dir, FileMode.Open, FileAccess.Read);
        // using var reader = new StreamReader(st);

        // var table = NetStat.Parse(reader).ToArray();
        // foreach (var row in table)
        // {
        //     foreach (var kvp in row)
        //     {
        //         Console.Write(kvp.Value.PadRight(60));
        //     }
        //     Console.WriteLine();
        // }
    }
}
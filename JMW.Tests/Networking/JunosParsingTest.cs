using JMW.Tests;
using NUnit.Framework;
using System.IO;

namespace JMW.Parsing.Tests;

[TestFixture]
public class JunosParsingTest : TestBase
{
    [Test]
    public void ParseTest()
    {
        var dir = Path.Combine(ExecutionPath, "Networking", "junos.txt");
        var junos = File.ReadAllText(dir);
        _ = new JMW.Networking.Parsers.JunosConfig.Parser().Parse(junos);
    }
}
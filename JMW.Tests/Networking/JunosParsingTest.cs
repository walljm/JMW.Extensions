using System.IO;
using System.Reflection;
using JMW.Networking.Junos.Parser;
using NUnit.Framework;

namespace JMW.Parsing.Tests
{
    [TestFixture]
    public class JunosParsingTest
    {
        [Test]
        public void ParseTest()
        {
            var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "Networking", "junos.txt");
            var junos = File.ReadAllText(dir);
            _ = new Parser().Parse(junos);
        }
    }
}
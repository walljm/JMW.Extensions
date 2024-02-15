using NUnit.Framework;
using System.IO;
using System.Reflection;

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
            _ = new JMW.Networking.Parsers.JunosConfig.Parser().Parse(junos);
        }
    }
}
using System.Collections.Generic;
using System.IO;

namespace JMW.Networking.Junos.Parser
{
    public interface IParser
    {
        IEnumerable<object[]> Parse(StreamReader reader);

        IEnumerable<object[]> Parse(string text);

        IEnumerable<Dictionary<string, object>> ParseNamed(string text);
    }
}
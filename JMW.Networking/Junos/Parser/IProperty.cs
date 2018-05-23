using System.IO;

namespace JMW.Networking.Junos.Parser
{
    public interface IProperty
    {
        string Name { get; }
        object Parse(StreamReader reader);
    }
}
using System.IO;

namespace JMW.Parsing.Compile
{
    public interface IProperty
    {
        object Parse(StreamReader reader);
    }
}
using System.Collections.Generic;
using System.IO;

namespace JMW.Parsing
{
    public interface ITagHandler
    {
        IEnumerable<object[]> Eval(Tag token, TextReader reader);

        void Validate(Tag tag, Token token);

        string Name { get; }
    }
}
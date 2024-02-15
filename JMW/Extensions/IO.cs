using JMW.Collections;
using System.IO;

namespace JMW.Extensions.IO;

public static class Extensions
{
    ///<summary>Returns the last <paramref name="lineCount"/> of lines from a <paramref name="reader"/>.</summary>
    ///<param name="reader">The reader to read from.</param>
    ///<param name="lineCount">The number of lines to return.</param>
    ///<returns>The last lines from the reader.</returns>
    public static string[] Tail(this TextReader reader, int lineCount)
    {
        var buffer = new RingBuffer<string>(lineCount);
        string line;
        while (null != (line = reader.ReadLine()))
        {
            buffer.Add(line);
        }

        return [.. buffer];
    }
}
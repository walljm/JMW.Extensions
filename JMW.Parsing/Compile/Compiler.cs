using JMW.Parsing.Handlers;
using System;

namespace JMW.Parsing.Compile;

public class Compiler
{
    /// <summary>
    /// Parse the parsing instruction, return an <see cref="IParser"/>.
    /// </summary>
    /// <param name="input">The parsing instructions as string</param>
    public static IParser Compile(string input)
    {
        var tags = new Parser().Parse(input);

        foreach (var tag in tags)
        {
            switch (tag.Name)
            {
                case Paragraph.NAME:
                    return new Paragraph(tag);

                case Table.NAME:
                    return new Table(tag);
            }
        }

        throw new NotSupportedException("Parsing Tag not supported. Top tag must be " + Paragraph.NAME + " or " + Table.NAME);
    }
}
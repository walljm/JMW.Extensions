using System;
using JMW.Parsing.Handlers;

namespace JMW.Parsing.Compile
{
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

            throw new NotSupportedException("Parsing Tags not supported.");
        }
    }
}
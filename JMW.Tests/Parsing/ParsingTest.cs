using System.IO;
using JMW.Parsing;
using NUnit.Framework;

namespace JMW.Types.Tests
{
    [TestFixture]
    public class ParsingTest
    {
        [Test]
        public void ParseTest()
        {
            var json = @"
            para {
                include: section {
                    start: startswith {s:[""foo""]}
                    stop: startswith {s:[""bar""]}
                }
                start: contains {s:[""start""] m:""i""} #this is a comment
                props:[
                    prop {
                        name:""jason""
                        line: contains {s:[""is""] m:""i""} #this is a comment
                        parsers:[
                            after {
                                s:[""is""]
                            }
                            to {
                                s:[""line""]
                            }

                        ]
                    }
                    prop {
                        name:""wall""
                        line: contains {s:[""an""] m:""i""} #this is a comment
                        split:""foo""
                        parsers:[
                            to {
                                s:[""is""]
                            }
                        ]
                    }
                ]
            }

";
            var text = @"
blah
blah
foo
start
 this is a line
 this is another line
start
 this is foo
 this is more foo
bar
start
 this is foo
 this is more foo
";
            var i = new Interpreter(new StringReader(text));
            foreach (var o in i.Eval(json))
            {
            }
        }
    }
}
using JMW.Parsing.Compile;
using NUnit.Framework;

namespace JMW.Parsing.Tests
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
                start: contains {s:[""start""]i} #this is a comment
                props:[
                    prop {
                        name:""jason""
                        line: contains {s:[""is""]i} #this is a comment
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
                        line: contains {s:[""an"" ""more""]i} #this is a comment
                        split:""foo""
                        parsers:[
                            to {
                                s:[""is"", ""more""]w
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
 this is 1 more foo
start
 this is foo
 this is 2 more foo
start
 this is foo
 this is 3 more foo
start
 this is foo
 this is 4 more foo
start
 this is foo
 this is 5 more foo
bar
start
 this is foo
 this is 6 more foo
";
            var i = Compiler.Compile(json);
            var output = string.Empty;

            foreach (var o in i.Parse(text))
            {
                output += o[0] + "|" + o[1] + ";";
            }

            Assert.AreEqual("is is a |start\n this;is is foo|\n this is 1 more;is is foo|\n this is 2 more;is is foo|\n this is 3 more;is is foo|\n this is 4 more;", output);
        }
    }
}
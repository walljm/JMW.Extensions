using System.Collections.Generic;
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

        [Test]
        public void ExpresssionTest_Regex()
        {
            var exp = new Expressions.Regex(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("i am Foo"));
            exp = new Expressions.Regex(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("i am Foo"));
            exp = new Expressions.Regex(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("i am Foo"));
            exp = new Expressions.Regex(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("i am Foo"));
        }

        [Test]
        public void ExpresssionTest_StartsWith()
        {
            var exp = new Expressions.StartsWith(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new Expressions.StartsWith(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("Foo bar"));
            exp = new Expressions.StartsWith(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("Foo bar"));
            exp = new Expressions.StartsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new Expressions.StartsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("bar foo bar"));
        }

        [Test]
        public void ExpresssionTest_EndsWith()
        {
            var exp = new Expressions.EndsWith(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp = new Expressions.EndsWith(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Expressions.EndsWith(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Expressions.EndsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("Foo barFoo"));
            exp = new Expressions.EndsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("bar foo Foo"));
        }

        [Test]
        public void ExpresssionTest_Count()
        {
            var exp = new Expressions.Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1-4,6-10"), "i");
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp = new Expressions.Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Expressions.Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsFalse(exp.Test("Foo barFoo foo foo foo"));
            exp = new Expressions.Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new Expressions.Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsTrue(exp.Test("bar foo Foo  foo foo  foo foo foo foo"));

            exp = new Expressions.Count(new List<string> { "foo", "bar" }, new Types.IntegerRangeCollection("1,4,6-8,10"), "i");
            Assert.IsFalse(exp.Test("bar foo Foo  foo foo  foo foo foo foo"));
        }


        [Test]
        public void ExpresssionTest_Look()
        {
            var exp = new Expressions.Look(new List<string> { "bar" }, "", new List<string> { "foo" }, "", new List<string> { "baz" }, "");
            Assert.IsTrue(exp.Test("foo bar baz"));
            exp = new Expressions.Look(new List<string> { "bar" }, "i", new List<string> { "Foo" }, "i", new List<string> { "Baz" }, "i");
            Assert.IsTrue(exp.Test("foo Bar baz"));
        }

    }
}
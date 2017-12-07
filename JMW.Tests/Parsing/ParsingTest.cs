using System.Collections.Generic;
using JMW.Parsing.Compile;
using JMW.Parsing.Expressions;
using JMW.Parsing.Extractors;
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
 this is 0 a line
 this is 0 another line
start
 this is 1 foo
 this is 1 more foo
start
 this is 2 foo
 this is 2 more foo
start
 this is 3 foo
 this is 3 more foo
start
 this is 4 foo
 this is 4 more foo
start
 this is 5 foo
 this is 5 more foo
bar
start
 this is 6 foo
 this is 6 more foo
";
            var i = Compiler.Compile(json);
            var output = string.Empty;

            foreach (var o in i.ParseNamed(text))
            {
                output += o["jason"] + "|" + o["wall"] + ";";
            }

            Assert.AreEqual("is is 0 a |start\n this;is is 1 foo|\n this;is is 2 foo|\n this;is is 3 foo|\n this;is is 4 foo|\n this;is is 5 foo|\n this;", output);
        }

        [Test]
        public void ExpresssionTest_Regex()
        {
            var exp = new Regex(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("i am Foo"));
            exp = new Regex(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("i am Foo"));
            exp = new Regex(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("i am Foo"));
            exp = new Regex(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("i am Foo"));
        }

        [Test]
        public void ExpresssionTest_StartsWith()
        {
            var exp = new StartsWith(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new StartsWith(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("Foo bar"));
            exp = new StartsWith(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("Foo bar"));
            exp = new StartsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new StartsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("bar foo bar"));
        }

        [Test]
        public void ExpresssionTest_EndsWith()
        {
            var exp = new EndsWith(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp = new EndsWith(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new EndsWith(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new EndsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("Foo barFoo"));
            exp = new EndsWith(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("bar foo Foo"));
        }

        [Test]
        public void ExpresssionTest_Contains()
        {
            var exp = new Expressions.Contains(new List<string> { "foo" }, "i");
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp = new Expressions.Contains(new List<string> { "foo" }, "in");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Expressions.Contains(new List<string> { "foo" }, "");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Expressions.Contains(new List<string> { "foo" }, "n");
            Assert.IsTrue(exp.Test("Foo barFoo"));
        }

        [Test]
        public void ExpresssionTest_Count()
        {
            var exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1-4,6-10"), "i");
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsFalse(exp.Test("Foo barFoo foo foo foo"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsTrue(exp.Test("bar foo Foo  foo foo  foo foo foo foo"));

            exp = new Count(new List<string> { "foo", "bar" }, new Types.IntegerRangeCollection("1,4,6-8,10"), "i");
            Assert.IsFalse(exp.Test("bar foo Foo  foo foo  foo foo foo foo"));
        }

        [Test]
        public void ExpresssionTest_Look()
        {
            var exp = new Look(new List<string> { "bar" }, "", new List<string> { "foo" }, "", new List<string> { "baz" }, "");
            Assert.IsTrue(exp.Test("foo bar baz"));
            exp = new Look(new List<string> { "bar" }, "i", new List<string> { "Foo" }, "i", new List<string> { "Baz" }, "i");
            Assert.IsTrue(exp.Test("foo Bar baz"));
        }

        [Test]
        public void ExpresssionTest_And()
        {
            var exp1 = new Expressions.Contains(new List<string> { "foo" }, "i");
            var exp2 = new Expressions.Contains(new List<string> { "bar" }, "i");
            var exp = new And(new List<IExpression> { exp1, exp2 });

            Assert.IsTrue(exp.Test("blah foo blah bar blah"));
            Assert.IsFalse(exp.Test("blah foo blah ar blah"));
        }

        [Test]
        public void ExpresssionTest_Or()
        {
            var exp1 = new Expressions.Contains(new List<string> { "foo" }, "i");
            var exp2 = new Expressions.Contains(new List<string> { "bar" }, "i");
            var exp = new Or(new List<IExpression> { exp1, exp2 });

            Assert.IsTrue(exp.Test("blah foo blah "));
            Assert.IsTrue(exp.Test("blah bar blah "));
            Assert.IsFalse(exp.Test("blah blah "));
        }

        [Test]
        public void Extractor_After()
        {
            var ext = new After(new List<string> { "foo" }, "i", -1);
            Assert.AreEqual("foo blah ", ext.Parse("blah foo blah "));

            ext = new After(new List<string> { "foo" }, "iw", -1);
            Assert.AreEqual(" blah ", ext.Parse("blah Foo blah "));
            ext = new After(new List<string> { "foo" }, "iwt", -1);
            Assert.AreEqual("blah", ext.Parse("blah Foo blah "));
            ext = new After(new List<string> { "foo" }, "iwtl", -1);
            Assert.AreEqual("what", ext.Parse("blah Foo blah foo what "));

            ext = new After(new List<string> { "foo" }, "iwtl", 2);
            Assert.AreEqual("what foo ho", ext.Parse("blah Foo blah foo what foo ho "));
        }

        [Test]
        public void Extractor_To()
        {
            var ext = new To(new List<string> { "foo" }, "i", -1);
            Assert.AreEqual("blah ", ext.Parse("blah foo blah "));

            ext = new To(new List<string> { "foo" }, "iw", -1);
            Assert.AreEqual("blah Foo", ext.Parse("blah Foo blah "));
            ext = new To(new List<string> { "foo" }, "iwt", -1);
            Assert.AreEqual("blah Foo", ext.Parse("blah Foo blah "));
            ext = new To(new List<string> { "foo" }, "iwtl", -1);
            Assert.AreEqual("blah Foo blah foo", ext.Parse("blah Foo blah foo what "));

            ext = new To(new List<string> { "foo" }, "iwtl", 2);
            Assert.AreEqual("blah Foo blah foo", ext.Parse("blah Foo blah foo what foo ho "));
        }

        [Test]
        public void Extractor_Split()
        {
            var ext = new Split(new List<string> { "foo" }, "i", 0);
            Assert.AreEqual("blah ", ext.Parse("blah foo blue "));
            ext = new Split(new List<string> { "foo" }, "it", 1);
            Assert.AreEqual("blue", ext.Parse("blah Foo blue "));
            ext = new Split(new List<string> { "foo" }, "ir", 1);
            Assert.AreEqual(" blue ", ext.Parse("blah foofoo blue "));
        }
    }
}
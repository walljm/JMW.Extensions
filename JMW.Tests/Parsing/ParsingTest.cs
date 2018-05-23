using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ParseParagraphTest()
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
                    prop {
                        name:""walljm""
                        line: or {
                                    contains {s:[""an"" ""more""]i}
                                    contains {s: [""an""]i}
                                 } #this is a comment
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
        public void ParseTableTest()
        {
            var json = @"
            table {
                row: contains {s:["" ""]i} #this is a comment
                props:[
                    prop {
                        name: ""C1""
                        parsers:[
                            col { i:""1"" }
                        ]
                    }
                ]
            }
";
            var text = @"
Jason Wall Col1 Col2
this  is   a    row
free  bird is   awesome
";
            var i = Compiler.Compile(json);
            var output = string.Empty;

            foreach (var o in i.Parse(text))
            {
                //output += o["jason"] + "|" + o["wall"] + ";";
            }

            Assert.AreEqual("is is 0 a |start\n this;is is 1 foo|\n this;is is 2 foo|\n this;is is 3 foo|\n this;is is 4 foo|\n this;is is 5 foo|\n this;", output);
        }


        [Test]
        public void ExpresssionTest_Base()
        {
            var exp_text = @" reg {
                                s:[""foo""]i #required
                              }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @" starts {
                                s:[""foo""]i #required
                              }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @" endswith {
                                s:[""foo""]i #required
                              }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @" contains {
                                s:[""foo""]i #required
                              }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @" count {
                                s:[""foo""]i #required
                                rng: ""1-4,6-10"" #required
                            }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @" look {
                                    a:[""bar""]i # optional
                                    s:[""foo""]ni #required
                                    b:[""baz""]i # optional
                                  }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @" and {
                                    contains {s:[""foo""]i}
                                    contains {s: [""bar""]i}
                                 }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            exp_text = @"or {
                                    contains {s:[""foo""]i}
                                    contains {s: [""bar""]i}
                                 }";
            Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
            try
            {
                exp_text = @"jason {a:[""Foo""]ni }";

                Expressions.Base.ToExpression(new Parser().Parse(exp_text).First());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
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

            var exp_text = @" reg {
                                s:[""foo""]i #required
                              }";
            exp = new Regex(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("i am Foo"));
            exp_text = @" regex {
                                s:[""foo""]in #required
                              }";
            exp = new Regex(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("i am Foo"));

            exp_text = @" reg {
                                s:[""foo""] #required
                              }";
            exp = new Regex(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("i am Foo"));

            exp_text = @" regex {
                                s:[""foo""]n #required
                              }";
            exp = new Regex(new Parser().Parse(exp_text).First());
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

            var exp_text = @" starts {
                                s:[""foo""]i #required
                              }";
            exp = new StartsWith(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo bar"));
            exp_text = @" startswith {
                                s:[""foo""]in #required
                              }";
            exp = new StartsWith(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo bar"));

            exp_text = @" starts {
                                s:[""foo""] #required
                              }";
            exp = new StartsWith(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo bar"));

            exp_text = @" startswith {
                                s:[""foo""]n #required
                              }";
            exp = new StartsWith(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo bar"));
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

            var exp_text = @" endswith {
                                s:[""foo""]i #required
                              }";
            exp = new EndsWith(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp_text = @" ends {
                                s:[""foo""]in #required
                              }";
            exp = new EndsWith(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo barFoo"));

            exp_text = @" endswith {
                                s:[""foo""] #required
                              }";
            exp = new EndsWith(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo barFoo"));

            exp_text = @" ends {
                                s:[""foo""]n #required
                              }";
            exp = new EndsWith(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo barFoo"));
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

            var exp_text = @" contains {
                                s:[""foo""]i #required
                              }";
            exp = new Expressions.Contains(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp_text = @" contains {
                                s:[""foo""]in #required
                              }";
            exp = new Expressions.Contains(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo barFoo"));

            exp_text = @" contains {
                                s:[""foo""] #required
                              }";
            exp = new Expressions.Contains(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo barFoo"));

            exp_text = @" contains {
                                s:[""foo""]n #required
                              }";
            exp = new Expressions.Contains(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo barFoo"));
        }

        [Test]
        public void ExpresssionTest_Count()
        {
            var exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1-4,6-10"), "i");
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp = new Count(new List<string> { "Foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "");
            Assert.IsFalse(exp.Test("Foo barFoo"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsFalse(exp.Test("Foo barFoo foo foo foo"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsTrue(exp.Test("Foo bar"));
            exp = new Count(new List<string> { "foo" }, new Types.IntegerRangeCollection("1,4,6-10"), "i");
            Assert.IsTrue(exp.Test("bar foo Foo  foo foo  foo foo foo foo"));

            exp = new Count(new List<string> { "foo", "bar" }, new Types.IntegerRangeCollection("1,4,6-8,10"), "i");
            Assert.IsFalse(exp.Test("bar foo Foo  foo foo  foo foo foo foo"));

            var exp_text = @"count {
                                s:[""foo""]i #required
                                rng: ""1-4,6-10"" #required
                            }";

            exp = new Count(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("Foo bar Foo"));
            exp_text = @"count {
                                s:[""foo""]i #required
                                rng: ""1,4,6-10"" #required
                            }";

            exp = new Count(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("Foo barFoo"));
            Assert.IsFalse(exp.Test("Foo barFoo foo foo foo"));
            Assert.IsTrue(exp.Test("Foo bar"));

            try
            {
                exp_text = @"count {
                                rng: ""1,4,6-10"" #required
                            }";

                exp = new Count(new Parser().Parse(exp_text).First());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                exp_text = @"count {
                                s:[""foo""]i #required
                            }";

                exp = new Count(new Parser().Parse(exp_text).First());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [Test]
        public void ExpresssionTest_Look()
        {
            var exp = new Look(new List<string> { "bar" }, "", new List<string> { "foo" }, "", new List<string> { "baz" }, "");
            Assert.IsTrue(exp.Test("foo bar baz"));
            exp = new Look(new List<string> { "bar" }, "i", new List<string> { "Foo" }, "i", new List<string> { "Baz" }, "i");
            Assert.IsTrue(exp.Test("foo Bar baz"));

            var exp_text = @"look {
                                    a:[""bar""]i # optional
                                    s:[""foo""]ni #required
                                    b:[""baz""]i # optional
                                  }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("bar foo baz"));
            exp_text = @"look {
                                    a:[""bar""]i # optional
                                    s:[""Foo""]ni #required
                                    b:[""Baz""]i # optional
                                  }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("foo Bar baz"));

            exp_text = @"look {
                                    s:[""Foo""]ni #required
                                    b:[""Baz""]i # optional
                                  }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("foo Bar baz"));

            exp_text = @"look {
                                    a:[""bar""]i # optional
                                    s:[""Foo""]ni #required
                                  }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsTrue(exp.Test("foo Bar baz"));

            exp_text = @"look {s:[""Foo""]ni }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("foo Bar baz"));

            exp_text = @"look {s:[""Woo""] }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("foo Bar baz"));

            exp_text = @"look {  a:[""bar""]i # optional
                                 s:[""Foo""]ni #required
                              }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("foo jar baz"));

            exp_text = @"look {  b:[""bar""]i # optional
                                 s:[""Foo""]ni #required
                              }";

            exp = new Look(new Parser().Parse(exp_text).First());
            Assert.IsFalse(exp.Test("foo jar baz"));
            try
            {
                exp_text = @"look {a:[""Foo""]ni }";

                exp = new Look(new Parser().Parse(exp_text).First());
                Assert.IsFalse(exp.Test("foo Bar baz"));
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [Test]
        public void ExpresssionTest_And()
        {
            var exp1 = new Expressions.Contains(new List<string> { "foo" }, "i");
            var exp2 = new Expressions.Contains(new List<string> { "bar" }, "i");
            var and1 = new And(new List<IExpression> { exp1, exp2 });

            Assert.IsTrue(and1.Test("blah foo blah bar blah"));
            Assert.IsFalse(and1.Test("blah foo blah ar blah"));

            var exp_text = @" and {
                                    contains {s:[""foo""]i}
                                    contains {s: [""bar""]i}
                                 }";
            var tags = new Parser().Parse(exp_text);
            var and2 = new And(tags.First());
            Assert.IsTrue(and2.Test("blah foo blah bar blah"));
            Assert.IsFalse(and2.Test("blah foo blah ar blah"));
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

            var exp_text = @" or {
                                    contains {s:[""foo""]i}
                                    contains {s: [""bar""]i}
                                 }";
            var tags = new Parser().Parse(exp_text);
            var or2 = new Or(tags.First());
            Assert.IsTrue(or2.Test("blah foo blah "));
            Assert.IsTrue(or2.Test("blah bar blah "));
            Assert.IsFalse(or2.Test("blah blah "));
        }

        [Test]
        public void ExtractorTest_Base()
        {
            var exp_text = @" split {
                                        s:[""foo""]irt #required
                                        i: ""2""  #required
                                    }
            ";
            Extractors.Base.ToExtractor(new Parser().Parse(exp_text).First());

            exp_text = @"  to {
                                        s:[""foo""]irt #required
                                        q: ""2""  #required
                                    }";
            Extractors.Base.ToExtractor(new Parser().Parse(exp_text).First());

            exp_text = @"after {
                                        s:[""foo""]irt #required
                                        q: ""2""  #required
                                    }";
            Extractors.Base.ToExtractor(new Parser().Parse(exp_text).First());

            try
            {
                exp_text = @"jason {a:[""Foo""]ni }";

                Extractors.Base.ToExtractor(new Parser().Parse(exp_text).First());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                exp_text = @"after {
                                        s:[""foo""]irt #required
                                        q: ""-1""  #required
                                    }";

                Extractors.Base.ToExtractor(new Parser().Parse(exp_text).First());
                Assert.Fail();
            }
            catch (ParseException)
            {
            }

            try
            {
                exp_text = @"split {
                                        s:[""foo""]irt #required
                                        i: ""-1""  #required
                                    }";

                Extractors.Base.ToExtractor(new Parser().Parse(exp_text).First());
                Assert.Fail();
            }
            catch (ParseException)
            {
            }
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

            try
            {
                ext = new Split(new List<string> { "foo" }, "", -1);
                Assert.AreEqual(" blue ", ext.Parse("blah foofoo blue "), "");
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                ext = new Split(new List<string> { "foo" }, "r", 2);
                ext.Parse("blah foofoo blue ");
                Assert.Fail();
            }
            catch (IndexOutOfRangeException)
            {
            }

            try
            {
                var exp_text = @" split {
                                        s:[""foo""]irt #required
                                        i: ""2""  #required
                                    }
            ";
                var exp = new Split(new Parser().Parse(exp_text).First());
                exp.Parse("blah foofoo blue ");
                Assert.Fail();
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        [Test]
        public void Extractor_Column()
        {
            var ext = new Column(1);
            ext.Positions = new List<Handlers.ColumnPosition>();
            ext.Positions.Add(new Handlers.ColumnPosition
            {
                Name = "Jason",
                Key = "Jason",
                Start = 2,
                Length = 5
            });
            ext.Positions.Add(new Handlers.ColumnPosition
            {
                Name = "Wall",
                Key = "Wall",
                Start = 8,
                Length = 4
            });
            Assert.AreEqual("Wall", ext.Parse("  Jason Wall "));

            var exp_text = @" col {
                                        i:""1"" #optional
                                        n: ""Jason"" #optional
                                    }";
            ext = new Column(new Parser().Parse(exp_text).First());
            ext.Positions = new List<Handlers.ColumnPosition>();
            ext.Positions.Add(new Handlers.ColumnPosition
            {
                Name = "Jason",
                Key = "Jason",
                Start = 2,
                Length = 5
            });
            ext.Positions.Add(new Handlers.ColumnPosition
            {
                Name = "Wall",
                Key = "Wall",
                Start = 8,
                Length = 4
            });
            Assert.AreEqual("Wall", ext.Parse("  Jason Wall "));

            try
            {
                exp_text = @" col {
                                        i:""3"" #optional
                                        n: ""Jason"" #optional
                                    }";
                ext = new Column(new Parser().Parse(exp_text).First());
                ext.Positions = new List<Handlers.ColumnPosition>();
                ext.Positions.Add(new Handlers.ColumnPosition
                {
                    Name = "Jason",
                    Key = "Jason",
                    Start = 2,
                    Length = 5
                });
                ext.Positions.Add(new Handlers.ColumnPosition
                {
                    Name = "Wall",
                    Key = "Wall",
                    Start = 8,
                    Length = 4
                });
                ext.Parse("  Jason Wall ");
                Assert.Fail();
            }
            catch (ArgumentException)
            { }

            try
            {
                exp_text = @" col {
                                        i:""3"" #optional
                                        n: ""Jason"" #optional
                                    }";
                ext = new Column(new Parser().Parse(exp_text).First());
                ext.Parse("  Jason Wall ");
                Assert.Fail();
            }
            catch (ArgumentException)
            { }
        }
    }
}
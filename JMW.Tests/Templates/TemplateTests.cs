using JMW.IO;
using JMW.Template.Tags;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace JMW.Template.Tests
{
    [TestFixture]
    public class InterpreterTests
    {
        [Test]
        public void TestTableVarIf1()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<var name=""jason"" value=""flaw""/>
<table name=""test"">
<foo/>\n
<if column=""bar"" exp=""x1 !='[var:jason]'""><bar/>\n</if>
<if column=""ip"" oct=""4"" exp=""x1==1""><ip/>\n</if>
<var:walljm />\n
<var:walljm exp=""x1.substr(0,2)"" />\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("blah\r\n1.1.1.1\r\nwooooo\r\nwo\r\nfloo\r\nblue\r\nwooooo\r\nwo\r\n", output);
        }

        [Test]
        public void TestTableVarIf2()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");
            var dt = new DataTable("test");
            dt.Columns.Add(new DataColumn("foo"));
            dt.Columns.Add(new DataColumn("bar"));
            dt.Columns.Add(new DataColumn("ip"));
            dt.Rows.Add("blah", "flaw", "1.1.1.1");
            dt.Rows.Add("floo", "blue", "1.2.3.4");
            var data = new TableData(dt);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<var name=""jason"" value=""flaw""/>
<table name=""test"">
<foo/>\n
<if column=""bar, foo"" exp=""x1 !='[var:jason]' && x2 == 'floo'""><bar/>\n</if>
<if column=""ip"" oct=""4"" exp=""x1==1""><ip/>\n</if>
<var:walljm />\n
<var:walljm exp=""x1.substr(0,2)"" />\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("blah\r\n1.1.1.1\r\nwooooo\r\nwo\r\nfloo\r\nblue\r\nwooooo\r\nwo\r\n", output);
        }

        [Test]
        public void TestTableVarIf3()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("test", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<if column=""ip,bar"" oct=""4"" exp=""x1==1""><ip/>\n</if>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The 'oct' attribute is currently incompatible with multiple arguments for the 'if' tag.", ex.Message);
            }
        }

        [Test]
        public void TestTableIf4()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("test", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<if column=""ip"" type='eq'><ip/>\n</if>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Missing attribute: 'value' Line: 2 Column: 1", ex.Message);
            }
        }

        [Test]
        public void TestTableIf5()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("test", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<if column=""ip"" value='foo'><ip/>\n</if>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Missing attribute: 'type' Line: 2 Column: 1", ex.Message);
            }
        }

        [Test]
        public void TestTableIf6()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("test", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<if column=""ip""><ip/>\n</if>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Missing attribute.  You must have either a 'type' and a 'value' attribute or an 'exp' attribute. Line: 2 Column: 1", ex.Message);
            }
        }

        [Test]
        public void TestTableIf7()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table>
<if column=""ip"" type='blah' value='foo'><ip/>\n</if>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid property value for 'type' property on 'if' tag. Allowed values: 'equals', 'notequals', 'contains', 'notcontains', 'startswith', 'notstartswith', 'endswith', 'notendswith', 'eq', 'neq', 'cnt', 'ncnt', 'strt', 'nstrt', 'ends', 'nends'. Line: 2 Column: 1", ex.Message);
            }
        }

        [Test]
        public void TestTableIf()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar,ip", "blah,flaw,1.1.1.1", "floo,blue,1.2.3.4"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            var output = runTableIf(sb, i, "equals", "flaw");
            Assert.AreEqual("flaw\r\n", output);
            output = runTableIf(sb, i, "eq", "flaw");
            Assert.AreEqual("flaw\r\n", output);

            output = runTableIf(sb, i, "notequals", "blue");
            Assert.AreEqual("flaw\r\n", output);
            output = runTableIf(sb, i, "neq", "blue");
            Assert.AreEqual("flaw\r\n", output);

            output = runTableIf(sb, i, "strt", "fl");
            Assert.AreEqual("flaw\r\n", output);
            output = runTableIf(sb, i, "startswith", "fl");
            Assert.AreEqual("flaw\r\n", output);

            output = runTableIf(sb, i, "nstrt", "bl");
            Assert.AreEqual("flaw\r\n", output);
            output = runTableIf(sb, i, "notstartswith", "bl");
            Assert.AreEqual("flaw\r\n", output);

            output = output = runTableIf(sb, i, "ends", "aw");
            Assert.AreEqual("flaw\r\n", output);
            output = output = runTableIf(sb, i, "endswith", "aw");
            Assert.AreEqual("flaw\r\n", output);

            output = output = runTableIf(sb, i, "nends", "ue");
            Assert.AreEqual("flaw\r\n", output);
            output = output = runTableIf(sb, i, "notendswith", "ue");
            Assert.AreEqual("flaw\r\n", output);

            output = output = runTableIf(sb, i, "contains", "la");
            Assert.AreEqual("flaw\r\n", output);
            output = output = runTableIf(sb, i, "cnt", "la");
            Assert.AreEqual("flaw\r\n", output);

            output = output = runTableIf(sb, i, "notcontains", "lu");
            Assert.AreEqual("flaw\r\n", output);
            output = output = runTableIf(sb, i, "ncnt", "lu");
            Assert.AreEqual("flaw\r\n", output);
        }

        private static string runTableIf(StringBuilder sb, Interpreter i, string type, string value)
        {
            sb.Clear();

            i.Eval(@"
<table name=""test"">
<if column=""bar"" value=""" + value + @""" type=""" + type + @"""><bar/>\n</if>
</table>
");

            return sb.ToString();
        }

        [Test]
        public void TestTableWhere()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"" where=""x1 != 'blah'"">
<foo/>\n
</table>
|||\n
<table name=""test"">
<foo/>\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("floo\r\nfloo\r\n|||\r\nblah\r\nfloo\r\nblah\r\nfloo\r\n", output);
        }

        [Test]
        public void TestTableDistinct()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"" distinct=""foo, bar"">
<foo/>\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("blah\r\nfloo\r\n", output);
        }

        [Test]
        public void TestTableOrderByAsc()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "ablah,flaw", "bfloo,blue", "cblah,flaw", "dfloo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"" orderby=""foo"">
<foo/>\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("ablah\r\nbfloo\r\ncblah\r\ndfloo\r\n", output);
        }

        [Test]
        public void TestTableOrderByDesc()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "ablah,flaw", "bfloo,blue", "cblah,flaw", "dfloo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"" orderby=""foo"" order=""desc"">
<foo/>\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("dfloo\r\ncblah\r\nbfloo\r\nablah\r\n", output);
        }

        [Test]
        public void TestAnonTable()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("", ["foo,bar", "blah,1.1.1.1", "floo,1.2.3.4", "blah,10.130.0.12", "floo,192.168.12.56"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<tab where=""x1 != 'blah'"">
<foo exp=""x1.substr(0,1)""/>\n
</tab>
|||\n
<table>
<bar oct=""3"" exp=""++x1""/>\n
</table>
");

            var output = sb.ToString();
            Assert.AreEqual("f\r\nf\r\n|||\r\n1.1.2.1\r\n1.2.4.4\r\n10.130.1.12\r\n192.168.13.56\r\n", output);
        }

        [Test]
        public void TestTab()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,1.1.1.1", "floo,1.2.3.4", "blah,10.130.0.12", "floo,192.168.12.56"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<tab name=""test"" where=""x1 != 'blah'"">
<foo exp=""x1.substr(0,1)""/>\n
</tab>
|||\n
<tab name=""test"">
<bar oct=""3"" exp=""++x1""/>\n
</tab>
");

            var output = sb.ToString();
            Assert.AreEqual("f\r\nf\r\n|||\r\n1.1.2.1\r\n1.2.4.4\r\n10.130.1.12\r\n192.168.13.56\r\n", output);
        }

        [Test]
        public void TestJoin()
        {
            var sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test2"" left_key_exp=""x1"" right_key=""baz"">
<left_foo/><left_bar/>\n
</join>
|||\n
<join left_table=""test1"" right_table=""test2"" left_key_exp=""x1"" right_key=""baz"">
<right_baz/><right_bar/>\n
</join>
");

                var output = sb.ToString();
                Assert.AreEqual("greenflaw\r\nblueblue\r\nredflaw\r\nblackblue\r\n|||\r\ngreenyellow\r\nbluepurple\r\nredpink\r\nblackbrown\r\n", output);
            }

            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test2"" left_key=""foo"" right_key_exp=""x1"">
<left_foo/><left_bar/>\n
</join>
|||\n
<join left_table=""test1"" right_table=""test2"" left_key_exp=""x1"" right_key=""baz"">
<right_baz/><right_bar/>\n
</join>
");

                var output = sb.ToString();
                Assert.AreEqual("greenflaw\r\nblueblue\r\nredflaw\r\nblackblue\r\n|||\r\ngreenyellow\r\nbluepurple\r\nredpink\r\nblackbrown\r\n", output);
            }

            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,1.1.1.1", "blue,1.1.1.1", "red,1.1.1.1", "black,1.1.1.1"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test2"" left_key=""foo"" right_key_exp=""x1"">
<left_foo/><left_bar oct=""3"" exp=""++x1""/>\n
</join>
");

                var output = sb.ToString();
                Assert.AreEqual("green1.1.2.1\r\nblue1.1.2.1\r\nred1.1.2.1\r\nblack1.1.2.1\r\n", output);
            }
        }

        [Test]
        public void TestJoinError1()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test3"" right_table=""test2"" left_key_exp=""x1"" right_key=""baz"">
<left_foo/><left_bar/>\n
</join>
");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Specified table: 'test3' in 'left_table' attribute does not exist.", ex.Message);
            }

            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test3"" left_key_exp=""x1"" right_key=""baz"">
<left_foo/><left_bar/>\n
</join>
");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Specified table: 'test3' in 'right_table' attribute does not exist.", ex.Message);
            }
        }

        [Test]
        public void TestJoinError2()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "foo,flaw", "foo,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test2"" left_key_exp=""x1"" right_key=""baz"">
<left_foo/><left_bar/>\n
</join>
");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Key expression does not produce a unique key.  Please provide a key expression that produces a unique key.  Duplicate key: 'foo'", ex.Message);
            }

            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test2"" right_key=""baz"">
<left_foo/><left_bar/>\n
</join>
");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Require attribute is missing. A 'left_key_exp' or a 'left_key' attribute is required.", ex.Message);
            }

            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                var data2 = new TableData("test2", ["baz,bar", "green,yellow", "blue,purple", "red,pink", "black,brown"]);
                var tbl2 = new Table(data2, i);
                i.AddHandler(tbl2);

                i.Eval(@"
<join left_table=""test1"" right_table=""test2"">
<left_foo/><left_bar/>\n
</join>
");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Require attribute is missing. A 'left_key_exp' or a 'left_key' attribute is required.", ex.Message);
            }
        }

        [Test]
        public void TestTableOutput()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
            "foo.txt".IfExists(File.Delete);

            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"">
<output filename=""foo.txt""><foo/>\n</output>
</table>
");
            var output = File.ReadAllText("foo.txt");
            Assert.AreEqual("blah\r\nfloo\r\nblah\r\nfloo\r\n", output);
            "foo.txt".IfExists(File.Delete);
        }

        [Test]
        public void TestTableOutputError1()
        {
            try

            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
                "foo.txt".IfExists(File.Delete);

                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<output filename=""foo.txt"" mode='blah'><foo/>\n</output>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Unknown attribute file for 'mode'.  Allowed values are: 'new' and 'append'", ex.Message);
            }
        }

        [Test]
        public void TestTableOutputError2()
        {
            try

            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
                "foo.txt".IfExists(File.Delete);

                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<output mode='blah'><foo/>\n</output>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The 'filename' attribute is required.", ex.Message);
            }
        }

        [Test]
        public void TestTableOutput_withNew()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
            "foo.txt".IfExists(File.Delete);

            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"">
<output filename=""foo.txt"" mode=""new""><foo/>\n</output>
</table>
");
            var output = File.ReadAllText("foo0.txt");
            Assert.AreEqual("blah\r\n", output);
            "foo0.txt".IfExists(File.Delete);

            output = File.ReadAllText("foo1.txt");
            Assert.AreEqual("floo\r\n", output);
            "foo1.txt".IfExists(File.Delete);

            output = File.ReadAllText("foo2.txt");
            Assert.AreEqual("blah\r\n", output);
            "foo2.txt".IfExists(File.Delete);

            output = File.ReadAllText("foo3.txt");
            Assert.AreEqual("floo\r\n", output);
            "foo3.txt".IfExists(File.Delete);
        }

        [Test]
        public void TestNamedIncludes()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<include define=""one"">
<foo/> | <bar/>\n
</include>

<table name=""test"">
<include name=""one""/>
</table>
");
            var output = sb.ToString();
            Assert.AreEqual("blah | flaw\r\nfloo | blue\r\nblah | flaw\r\nfloo | blue\r\n", output);
        }

        [Test]
        public void TestNamedIncludesError1()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);

                var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<include name='foo'/>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The <include> tag referenced named include 'foo' which does not exist in the template.", ex.Message);
            }
        }

        [Test]
        public void TestNamedIncludesError2()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);

                var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<include/>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The <include> tag requires either a 'name' attribute or a 'path' attribute.", ex.Message);
            }
        }

        [Test]
        public void TestPathIncludes()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));

            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"">
<include path=""" + Path.Combine(".", "Templates", "testpathinclude.tmpl") + @"""/>
</table>
");
            var output = sb.ToString();
            Assert.AreEqual("blah | flaw\r\nfloo | blue\r\nblah | flaw\r\nfloo | blue\r\n", output);
        }

        [Test]
        public void TestPathIncludesError()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(InterpreterTests)).Location));
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);

                var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
                var tbl = new Table(data, i);
                i.AddHandler(tbl);

                i.Eval(@"
<table name=""test"">
<include path=""" + Path.Combine(".", "Templates", "asdf.tmpl") + @"""/>
</table>
");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("The <include> tag refers to a template which does not exist"));
            }
        }

        [Test]
        public void TestLookup1()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data1 = new TableData("test1", ["foo,bar", "green,flaw", "blue,blue", "red,flaw", "black,blue"]);
            var tbl1 = new Table(data1, i);

            i.AddHandler(tbl1);

            i.Eval(@"
<lookup table=""test1"" key_exp=""x1"">
<row key=""blue"">
<foo/><bar/>\n
</row>
<row key=""black"">
<foo/><bar/>\n
</row>
</lookup>
");

            var output = sb.ToString();
            Assert.AreEqual("blueblue\r\nblackblue\r\n", output);
        }

        [Test]
        public void TestLookup2()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data1 = new TableData("test1", ["foo,bar,ip", "green,flaw,1.1.1.1", "blue,blue,1.1.1.1", "red,flaw,1.1.1.1", "black,blue,1.1.1.1"]);
            var tbl1 = new Table(data1, i);

            i.AddHandler(tbl1);

            i.Eval(@"
<lookup table=""test1"" key_col=""bar"">
<row key=""blue"">
<foo/><bar/><ip oct=""3"" exp=""++x1""/>\n
</row>
</lookup>
");

            var output = sb.ToString();
            Assert.AreEqual("blueblue1.1.2.1\r\nblackblue1.1.2.1\r\n", output);
        }

        [Test]
        public void TestLookupError1()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar,ip", "green,flaw,1.1.1.1", "blue,blue,1.1.1.1", "red,flaw,1.1.1.1", "black,blue,1.1.1.1"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                i.Eval(@"
<lookup table=""test2"" key_col=""bar"">
<row key=""blue"">
<foo/><bar/><ip oct=""3"" exp=""++x1""/>\n
</row>
</lookup>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Specified table: 'test2' in 'table' attribute does not exist.", ex.Message);
            }
        }

        [Test]
        public void TestLookupError2()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar,ip", "green,flaw,1.1.1.1", "blue,blue,1.1.1.1", "red,flaw,1.1.1.1", "black,blue,1.1.1.1"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                i.Eval(@"
<lookup table=""test1"">
<row key=""blue"">
<foo/><bar/><ip oct=""3"" exp=""++x1""/>\n
</row>
</lookup>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Require attribute is missing. A 'key_exp' or 'key_col' attribute is required.", ex.Message);
            }
        }

        [Test]
        public void TestLookupRowError1()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("walljm", "wooooo");

                var data1 = new TableData("test1", ["foo,bar,ip", "green,flaw,1.1.1.1", "blue,blue,1.1.1.1", "red,flaw,1.1.1.1", "black,blue,1.1.1.1"]);
                var tbl1 = new Table(data1, i);

                i.AddHandler(tbl1);

                i.Eval(@"
<lookup table=""test1"" key_col='foo'>
<row>
<foo/><bar/><ip oct=""3"" exp=""++x1""/>\n
</row>
</lookup>
");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The 'key' attribute is required.", ex.Message);
            }
        }

        [Test]
        public void TestVersion()
        {
            var sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"
version: <version/>
");

                var output = sb.ToString();
                Assert.IsTrue(output.Contains("version: Templating Engine Version "));
            }
            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"
version: <version prefix=""TE: "" />
");

                var output = sb.ToString();
                Assert.IsTrue(output.Contains("version: TE: "));
            }

            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"
version: <version prefix=""TE: "" exp=""x1.substr(0, 7)""/>
");

                var output = sb.ToString();
                Assert.IsTrue(output.Contains("version: TE: "));
            }
        }

        [Test]
        public void TestTimestamp()
        {
            var sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"
datetime: <timestamp/>
");

                var output = sb.ToString();
                Assert.IsTrue(output.StartsWith("datetime: " + DateTime.Now.ToString("M/d/yyyy hh:mm")));
            }

            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"
datetime: <timestamp format=""M-d-yy""/>
");

                var output = sb.ToString();
                Assert.AreEqual(output, "datetime: " + DateTime.Now.ToString("M-d-yy"));
            }

            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"
datetime: <timestamp format=""M-d-yy hh:mm:ss"" exp=""x1.substr(0,x1.indexOf(' '));""/>
");

                var output = sb.ToString();
                Assert.AreEqual(output, "datetime: " + DateTime.Now.ToString("M-d-yy"));
            }
        }

        [Test]
        public void TestTimestampError()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);

                i.Eval(@"datetime: <timestamp format=""123!@*(&^$@(^%"" />");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Unable to interpret tag 'timestamp' with format: '123!@*(&^$@(^%'. Error Message: Input string was not in a correct format.", ex.Message);
            }
        }

        [Test]
        public void TestDefaultHandler()
        {
            var sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr);

                i.Eval(@"<jason/>");

                var output = sb.ToString();
                Assert.AreEqual(string.Empty, output);
            }

            sb = new StringBuilder();
            using (var wr = new StringWriter(sb))
            {
                var i = new Interpreter(wr)
                {
                    DefaultHandler = new Functional.Optional<Handler>((t, h) => { h.OutputStream.Write("jason"); })
                };

                i.Eval(@"<jason/>");

                var output = sb.ToString();
                Assert.AreEqual("jason", output);
            }
        }

        [Test]
        public void TestAdditionalHandler()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(new Parser(), wr, new List<ITagHandler> { new TestHandler() });

            i.Eval(@"<test>jason</test>");

            var output = sb.ToString();
            Assert.AreEqual("JASON", output);
        }

        [Test]
        public void TestVar1()
        {
            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("IP", "1.1.1.1");

                i.Eval(@"<var:IP oct='2' exp='x+5' store='atrue' print='false'/>");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The 'store' attribute value 'atrue' was invalid. Value must be either 'true' or 'false'.", ex.Message);
            }

            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("IP", "1.1.1.1");

                i.Eval(@"<var:IP oct='2' exp='x+5' store='true' print='afalse'/>");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The 'print' attribute value 'afalse' was invalid. Value must be either 'true' or 'false'.", ex.Message);
            }

            try
            {
                var sb = new StringBuilder();
                using var wr = new StringWriter(sb);
                var i = new Interpreter(wr);
                i.AddVariable("IP", "1.1.1.1");

                i.Eval(@"<var />");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("'var' tag requires both 'name' and 'value' attributes.", ex.Message);
            }

            var sb1 = new StringBuilder();
            using (var wr = new StringWriter(sb1))
            {
                var i = new Interpreter(wr);
                i.AddVariable("IP", "1.1.1.1");

                i.Eval(@"<var:ip exp='x1+10' oct='4' />");
                Assert.AreEqual("1.1.1.11", sb1.ToString());
            }

            sb1 = new StringBuilder();
            using (var wr = new StringWriter(sb1))
            {
                var i = new Interpreter(wr);
                i.AddVariable("IP", "1.1.1.1");

                i.Eval(@"<var:ip store='true' value='2.2.2.2'/>");
                Assert.AreEqual("2.2.2.2", sb1.ToString());
            }
        }

        [Test]
        public void TestVar2()
        {
            try
            {
                var v = new Variable("var");
                v.AddVariable("jason", "foo");
                v.AddVariable("jason", "foo");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Variable: 'jason' already exists.", ex.Message);
            }
        }

        [Test]
        public void TestTransform1()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"">
<transform columns='foo,bar' exp='x1+""|""+x2' />\n
</table>
");
            var output = sb.ToString();
            Assert.AreEqual("blah|flaw\r\nfloo|blue\r\nblah|flaw\r\nfloo|blue\r\n", output);
        }

        [Test]
        public void TestTransform2()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"">
<trans columns='foo,bar' exp='x1+""|""+x2' />\n
</table>
");
            var output = sb.ToString();
            Assert.AreEqual("blah|flaw\r\nfloo|blue\r\nblah|flaw\r\nfloo|blue\r\n", output);
        }

        [Test]
        public void TestTransform3()
        {
            var sb = new StringBuilder();
            using var wr = new StringWriter(sb);
            var i = new Interpreter(wr);
            i.AddVariable("walljm", "wooooo");

            var data = new TableData("test", ["foo,bar", "blah,flaw", "floo,blue", "blah,flaw", "floo,blue"]);
            var tbl = new Table(data, i);
            i.AddHandler(tbl);

            i.Eval(@"
<table name=""test"">
<tr columns='foo,bar' exp='x1+""|""+x2' />\n
</table>
");
            var output = sb.ToString();
            Assert.AreEqual("blah|flaw\r\nfloo|blue\r\nblah|flaw\r\nfloo|blue\r\n", output);
        }
    }

    public class TestHandler : TagHandlerBase
    {
        public override string TagName { get; } = "test";
        public override HashSet<string> ALLOWEDPROPS { get; } = [];

        public override void Handler(Tag token, Interpreter interp)
        {
            interp.OutputStream.Write(token.Children.First().TokenText.ToUpper());
        }
    }
}
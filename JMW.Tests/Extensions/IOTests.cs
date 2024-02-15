using NUnit.Framework;
using System;
using System.IO;

namespace JMW.Extensions.IO.Tests
{
    [TestFixture]
    public class IOTests
    {
        [Test]
        public void TailTest()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..","..","..","Extensions","lines.txt");
            using var r = new StreamReader(path);
            var last = r.Tail(5);
            Assert.That(5, Is.EqualTo(last.Length));
            Assert.That("foo bar 6", Is.EqualTo(last[0]));
            Assert.That("foo bar 7", Is.EqualTo(last[1]));
            Assert.That("foo bar 8", Is.EqualTo(last[2]));
            Assert.That("foo bar 9", Is.EqualTo(last[3]));
            Assert.That("foo bar 10", Is.EqualTo(last[4]));
        }

        [Test]
        public void TailTestLong()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..","..","..","Extensions","lines_long.txt");
            using var r = new StreamReader(path);
            var last = r.Tail(5);
            Assert.That(5, Is.EqualTo(last.Length));
            Assert.That("foo bar 6", Is.EqualTo(last[0]));
            Assert.That("foo bar 7", Is.EqualTo(last[1]));
            Assert.That("foo bar 8", Is.EqualTo(last[2]));
            Assert.That("foo bar 9", Is.EqualTo(last[3]));
            Assert.That("foo bar 10", Is.EqualTo(last[4]));
        }
    }
}
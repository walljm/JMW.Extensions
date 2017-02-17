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
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Extensions\\lines.txt");
            using (var r = new StreamReader(path))
            {
                var last = r.Tail(5);
                Assert.AreEqual(5, last.Length);
                Assert.AreEqual("foo bar 6", last[0]);
                Assert.AreEqual("foo bar 7", last[1]);
                Assert.AreEqual("foo bar 8", last[2]);
                Assert.AreEqual("foo bar 9", last[3]);
                Assert.AreEqual("foo bar 10", last[4]);
            }
        }

        [Test]
        public void TailTestLong()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Extensions\\lines_long.txt");
            using (var r = new StreamReader(path))
            {
                var last = r.Tail(5);
                Assert.AreEqual(5, last.Length);
                Assert.AreEqual("foo bar 6", last[0]);
                Assert.AreEqual("foo bar 7", last[1]);
                Assert.AreEqual("foo bar 8", last[2]);
                Assert.AreEqual("foo bar 9", last[3]);
                Assert.AreEqual("foo bar 10", last[4]);
            }
        }
    }
}
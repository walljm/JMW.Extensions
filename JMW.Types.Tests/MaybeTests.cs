﻿using NUnit.Framework;
using System;

namespace JMW.Types.Functional.Tests
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void OptionalConstructorTest1()
        {
            string v = null;
            var c = new Maybe<string>(v);
            Assert.AreEqual(false, c.Success);
        }

        [Test]
        public void ComparisonsTest1()
        {
            var m1 = new Maybe<string>("blah");
            var m2 = new Maybe<string>("blah");

            var m3 = new Maybe<string>((string)null);
            var m4 = new Maybe<string>("blah1");
            var m5 = new Maybe<string>((Exception)null);
            Maybe<string> m6 = null;

            Assert.That(m1 == m2);
            Assert.That(m1 != m3);
            Assert.That(m1 != "hohum");
            Assert.That(m1 == "blah");
            Assert.That(m1 != m5);
            Assert.That(m1 != m4);
            Assert.That(null == null);
            Assert.That((Maybe<string>)null != "woo");
            Assert.That((Maybe<string>)null == (string)null);
            Assert.That("woo" != (Maybe<string>)null);
            Assert.That(m6 == (Maybe<string>)null);
            Assert.That("blah" == m1);
            Assert.That(!m5.Equals("woo"));
            Assert.That(m1 != m6);
        }
    }
}
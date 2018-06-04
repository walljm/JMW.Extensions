using System;
using JMW.Extensions.DateAndTime;
using NUnit.Framework;

namespace JMW.Extensions.DateAndTime.Tests
{
    [TestFixture]
    public class DateAndTimeTests
    {
        [Test]
        public void GetElapsedTimeTest()
        {
            Assert.AreEqual("01:01:01", new DateTime(2018, 01, 01, 01, 01, 01).GetElapsedTime(new DateTime(2018, 1, 1)));
        }
    }
}
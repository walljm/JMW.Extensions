using NUnit.Framework;
using System;

namespace JMW.Extensions.Numbers.Tests
{
    [TestFixture]
    public class NumbersTests
    {
        [Test]
        public void IsIntTest()
        {
            Assert.IsTrue("1".IsInt());
            Assert.IsTrue("11234".IsInt());
            Assert.IsTrue("4321341".IsInt());
            Assert.IsTrue("1431234121".IsInt());
            Assert.IsFalse("143.1234121".IsInt());
            Assert.IsFalse("14a".IsInt());
        }

        [Test]
        public void IsDoubleTest()
        {
            Assert.IsTrue("1".IsDouble());
            Assert.IsTrue("11234".IsDouble());
            Assert.IsTrue("4321341".IsDouble());
            Assert.IsTrue("1123412341234".IsDouble());
            Assert.IsTrue("1431234121".IsDouble());
            Assert.IsTrue("143.1234121".IsDouble());
            Assert.IsFalse("14a".IsDouble());
        }

        [Test]
        public void IsNotNullTest()
        {
            string test = null;

            Assert.IsTrue("blah".IsNotNull());
            Assert.IsFalse(test.IsNotNull());
        }

        [Test]
        public void IsNullTest()
        {
            string test = null;

            Assert.IsFalse("blah".IsNull());
            Assert.IsTrue(test.IsNull());
        }

        [Test]
        public void ToIntTest()
        {
            Assert.AreEqual(1, "1".ToInt());
            Assert.AreEqual(11234, "11234".ToInt());
            Assert.AreEqual(4321341, "4321341".ToInt());
            Assert.AreEqual(Int32.MaxValue, Int32.MaxValue.ToString().ToInt());
            Assert.AreEqual(1431234121, "1431234121".ToInt());
        }

        [Test]
        public void ToIntOrNeg1Test()
        {
            Assert.AreEqual(-1, "basdf".ToIntOrNeg1());
            Assert.AreEqual(Int32.MaxValue, Int32.MaxValue.ToString().ToIntOrNeg1());
        }
    }
}
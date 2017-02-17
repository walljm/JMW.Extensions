using NUnit.Framework;

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
            Assert.IsTrue("1123412341234".IsInt());
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

            Assert.IsFalse("blah".IsNotNull());
            Assert.IsTrue(test.IsNotNull());
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
            Assert.Fail();
        }

        [Test]
        public void ToIntTest1()
        {
            Assert.Fail();
        }

        [Test]
        public void ToIntTest2()
        {
            Assert.Fail();
        }

        [Test]
        public void ToIntOrNeg1Test()
        {
            Assert.Fail();
        }

        [Test]
        public void ToInt64OrNeg1Test()
        {
            Assert.Fail();
        }

        [Test]
        public void ToInt64Test()
        {
            Assert.Fail();
        }

        [Test]
        public void ToInt64orNeg1Test()
        {
            Assert.Fail();
        }

        [Test]
        public void ToFloatTest()
        {
            Assert.Fail();
        }

        [Test]
        public void ToDoubleTest()
        {
            Assert.Fail();
        }

        [Test]
        public void CollapseIntsToRangesTest()
        {
            Assert.Fail();
        }

        [Test]
        public void CollapseIntsToRangesTest1()
        {
            Assert.Fail();
        }

        [Test]
        public void NearlyEqualTest()
        {
            Assert.Fail();
        }
    }
}
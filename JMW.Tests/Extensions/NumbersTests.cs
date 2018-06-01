using System;
using System.Collections.Generic;
using JMW.Types;
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
        public void ToDoubleTest()
        {
            Assert.AreEqual((double)1, "1".ToDouble());
            Assert.AreEqual((double)11234, "11234".ToDouble());
            Assert.AreEqual((double)4321341, "4321341".ToDouble());
            Assert.AreEqual((double)1431234121, "1431234121".ToDouble());

            Assert.AreEqual(-1, "1431234121aasdf".ToDoubleOrNeg1());

            Assert.AreEqual((double)1, 1.ToDouble());
            Assert.AreEqual((double)11234, 11234.ToDouble());
            Assert.AreEqual((double)4321341, 4321341.ToDouble());
            Assert.AreEqual((double)1431234121, 1431234121.ToDouble());

            Assert.AreEqual((double)1, ((long)1).ToDouble());
            Assert.AreEqual((double)11234, ((long)11234).ToDouble());
            Assert.AreEqual((double)4321341, ((long)4321341).ToDouble());
            Assert.AreEqual((double)1431234121, ((long)1431234121).ToDouble());
        }

        [Test]
        public void ToLongTest()
        {
            Assert.AreEqual((long)1, "1".ToLong());
            Assert.AreEqual((long)11234, "11234".ToLong());
            Assert.AreEqual((long)4321341, "4321341".ToLong());
            Assert.AreEqual((long)1431234121, "1431234121".ToLong());

            Assert.AreEqual((long)1, ((decimal)1).ToLong());
            Assert.AreEqual((long)11234, ((decimal)11234).ToLong());
            Assert.AreEqual((long)4321341, ((decimal)4321341).ToLong());
            Assert.AreEqual((long)1431234121, ((decimal)1431234121).ToLong());

            Assert.AreEqual((long)1, ((double)1).ToLong());
            Assert.AreEqual((long)11234, ((double)11234).ToLong());
            Assert.AreEqual((long)4321341, ((double)4321341).ToLong());
            Assert.AreEqual((long)1431234121, ((double)1431234121).ToLong());

            Assert.AreEqual((long)1, "1".ToInt64());
            Assert.AreEqual((long)11234, "11234".ToInt64());
            Assert.AreEqual((long)4321341, "4321341".ToInt64());
            Assert.AreEqual((long)1431234121, "1431234121".ToInt64());

            Assert.AreEqual(-1, "1431234121asdf".ToInt64OrNeg1());
            Assert.AreEqual(-1, "143123412000001b".ToInt64OrNeg1());
            Assert.AreEqual(1431234121, "1431234121".ToInt64OrNeg1());
            Assert.AreEqual(1000, "1k".ToInt64OrNeg1());
        }

        [Test]
        public void ToFloatTest()
        {
            Assert.AreEqual(1f, "1".ToFloat());
            Assert.AreEqual(11234f, "11234".ToFloat());
            Assert.AreEqual(4321341f, "4321341".ToFloat());
            Assert.AreEqual(1431234121f, "1431234121".ToFloat());

            float? fl = "143123412asdf1".ToFloat();
            Assert.AreEqual(null, fl);

            Assert.AreEqual(-1, "1431234121sdf".ToFloatOrNeg1());
        }

        [Test]
        public void ToIntTest()
        {
            Assert.AreEqual(1, "1".ToInt());
            Assert.AreEqual(11234, "11234".ToInt());
            Assert.AreEqual(4321341, "4321341".ToInt());
            Assert.AreEqual(Int32.MaxValue, Int32.MaxValue.ToString().ToInt());
            Assert.AreEqual(1431234121, "1431234121".ToInt());

            Assert.AreEqual(1, "1".ToIntFast());
            Assert.AreEqual(11234, "11234".ToIntFast());
            Assert.AreEqual(4321341, "4321341".ToIntFast());
            Assert.AreEqual(Int32.MaxValue, Int32.MaxValue.ToString().ToIntFast());
            Assert.AreEqual(1431234121, "1431234121".ToIntFast());

            Assert.AreEqual(1, 1.0.ToInt());
            Assert.AreEqual(11234, 11234.0.ToInt());
            Assert.AreEqual(4321341, 4321341.0.ToInt());
            Assert.AreEqual(1431234121, 1431234121.0.ToInt());

            Assert.AreEqual(1, ((decimal)1).ToInt());
            Assert.AreEqual(11234, ((decimal)11234).ToInt());
            Assert.AreEqual(4321341, ((decimal)4321341).ToInt());
            Assert.AreEqual(1431234121, ((decimal)1431234121).ToInt());

            Assert.AreEqual(1, ((double)1).ToInt());
            Assert.AreEqual(11234, ((double)11234).ToInt());
            Assert.AreEqual(4321341, ((double)4321341).ToInt());
            Assert.AreEqual(1431234121, ((double)1431234121).ToInt());
        }

        [Test]
        public void ToIntOrNeg1Test()
        {
            Assert.AreEqual(-1, "basdf".ToIntOrDefaultFast(-1));
            Assert.AreEqual(Int32.MaxValue, Int32.MaxValue.ToString().ToIntOrDefaultFast(-1));

            Assert.AreEqual(-1, "basdf".ToIntOrNeg1());
            Assert.AreEqual(Int32.MaxValue, Int32.MaxValue.ToString().ToIntOrNeg1());
            Assert.AreEqual(1000, "1k".ToIntOrNeg1());
            Assert.AreEqual(-1, "143123412000001b".ToIntOrNeg1());
        }

        [Test]
        public void IsLongTest()
        {
            Assert.IsTrue("1".IsLong());
            Assert.IsTrue("11234".IsLong());
            Assert.IsTrue("4321341".IsLong());
            Assert.IsTrue("1123412341234".IsLong());
            Assert.IsTrue("1431234121".IsLong());
            Assert.IsFalse("14a".IsLong());
            Assert.IsTrue(long.MaxValue.ToString().IsLong());
            Assert.IsTrue(long.MinValue.ToString().IsLong());
        }

        [Test]
        public void CollapseIntsToRangesTest()
        {
            Assert.AreEqual("1, 4-8, 11", new List<string> { "1", "4", "5", "6", "7", "8", "11" }.CollapseIntsToRanges());
            Assert.AreEqual("1", new List<string> { "1" }.CollapseIntsToRanges());
        }

        [Test]
        public void CollapseLongtsToRangesTest()
        {
            Assert.AreEqual("1, 4-8, 11", new List<string> { "1", "4", "5", "6", "7", "8", "11" }.CollapseLongsToRanges());
            Assert.AreEqual("1", new List<string> { "1" }.CollapseLongsToRanges());
        }


        [Test]
        public void CollapseIntegerRangesToRangesTest()
        {
            var expected = new List<IntegerRange>
            {
                new IntegerRange(1,1),
                new IntegerRange(4,8),
                new IntegerRange(11,11)
            };

            Assert.AreEqual(expected.Count, new List<int> { 1, 4, 5, 6, 7, 8, 11 }.CollapseIntsToIntegerRanges().Count);
            Assert.AreEqual(expected[1].Start, new List<int> { 1, 4, 5, 6, 7, 8, 11 }.CollapseIntsToIntegerRanges()[1].Start);
            Assert.AreEqual(expected[1].Stop, new List<int> { 1, 4, 5, 6, 7, 8, 11 }.CollapseIntsToIntegerRanges()[1].Stop);
        }
    }
}
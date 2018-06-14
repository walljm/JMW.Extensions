using System.Collections.Generic;
using JMW.Types;
using NUnit.Framework;

namespace JMW.Extensions.Tests
{
    [TestFixture]
    public class IntegerRangeCollectionTests
    {
        [Test]
        public void IntegerRangeTest1()
        {
            // arrange/act
            var foo = new IntegerRangeCollection("1-4,5,7,10,8, 12-15, 8-10, 19-34, 20-24, 45-50, 40-46");

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-5,7-10,12-15,19-34,40-50"));
            Assert.That(foo.IsInRange(9), Is.EqualTo(true));
            Assert.That(foo.IsInRange(3), Is.EqualTo(true));
            Assert.That(foo.IsInRange(13), Is.EqualTo(true));
            Assert.That(foo.IsInRange(0), Is.EqualTo(false));

            Assert.That(foo.IsInRange("13"), Is.EqualTo(true));
            Assert.That(foo.IsInRange("0"), Is.EqualTo(false));

            Assert.That(foo.IsInRange(16), Is.EqualTo(false));
            Assert.That(foo.Contains(16), Is.EqualTo(false));
            Assert.That(foo.Contains(10), Is.EqualTo(true));
            Assert.That(foo.Contains("16"), Is.EqualTo(false));
            Assert.That(foo.Contains("10"), Is.EqualTo(true));
            Assert.That(new IntegerRange("8-12").Contains("10"), Is.EqualTo(true));
            Assert.That(new IntegerRange(new IntegerRange("8-12")).Contains("10"), Is.EqualTo(true));
        }

        [Test]
        public void TestAddIntegerRanges()
        {
            // arrange/act
            var foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.Add(new IntegerRange("3-11"));

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19"));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<IntegerRange> { new IntegerRange("3-9"), new IntegerRange("12-20") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-10,12-20"));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<IntegerRange> { new IntegerRange("3-6"), new IntegerRange("11"), new IntegerRange("35") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-6,8,10-15,19,35"));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<IntegerRange> { new IntegerRange("3-11"), new IntegerRange("13-14"), new IntegerRange("19-21") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19-21"));
        }

        [Test]
        public void LongRangeTest1()
        {
            // arrange/act
            var foo = new LongRangeCollection("1-4,5,7,10,8, 12-15, 8-10, 19-34, 20-24, 45-50, 40-46");

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-5,7-10,12-15,19-34,40-50"));
            Assert.That(foo.IsInRange(9), Is.EqualTo(true));
            Assert.That(foo.IsInRange(3), Is.EqualTo(true));
            Assert.That(foo.IsInRange(13), Is.EqualTo(true));
            Assert.That(foo.IsInRange(0), Is.EqualTo(false));
            Assert.That(foo.IsInRange(16), Is.EqualTo(false));
        }

        [Test]
        public void TestAddLongRanges()
        {
            // arrange/act
            var foo = new LongRangeCollection("1-4,10,8, 12-15,19");
            foo.Add(new LongRange("3-11"));

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19"));

            foo = new LongRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<LongRange> { new LongRange("3-9"), new LongRange("12-20") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-10,12-20"));

            foo = new LongRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<LongRange> { new LongRange("3-6"), new LongRange("11"), new LongRange("35") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-6,8,10-15,19,35"));

            foo = new LongRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<LongRange> { new LongRange("3-11"), new LongRange("13-14"), new LongRange("19-21") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19-21"));
        }
    }
}
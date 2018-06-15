using System.Collections.Generic;
using JMW.Types;
using NUnit.Framework;

namespace JMW.Extensions.Tests
{
    [TestFixture]
    public class IntegerRangeCollectionTests
    {
        [Test]
        public void RangeTest1()
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
        public void RangeConstructors()
        {
            var foo = new IntegerRangeCollection("1-4,5,7,10,8, 12-15, 8-10, 19-34, 20-24, 45-50, 40-46");
            Assert.That(foo.ToString(), Is.EqualTo("1-5,7-10,12-15,19-34,40-50"));
            
            foo = new IntegerRangeCollection(new List<IntegerRange>{
                new IntegerRange(1,5),
                new IntegerRange(7,10),
                new IntegerRange(12,15),
                new IntegerRange(19,34),
                new IntegerRange(40,50)
            });
            Assert.That(foo.ToString(), Is.EqualTo("1-5,7-10,12-15,19-34,40-50"));

            var foo1 = new LongRangeCollection("1-4,5,7,10,8, 12-15, 8-10, 19-34, 20-24, 45-50, 40-46");
            Assert.That(foo1.ToString(), Is.EqualTo("1-5,7-10,12-15,19-34,40-50"));
            foo1 = new LongRangeCollection(new List<LongRange>{
                new LongRange(1,5),
                new LongRange(7,10),
                new LongRange(12,15),
                new LongRange(19,34),
                new LongRange(40,50)
            });
            Assert.That(foo.ToString(), Is.EqualTo("1-5,7-10,12-15,19-34,40-50"));

            
        }

        [Test]
        public void TestAddIntegerRanges()
        {
            // arrange/act
            var foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.Add(new IntegerRange("3-11"));

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19"));

            var foo1 = new LongRangeCollection("1-4,10,8, 12-15,19");
            foo1.AddRange(new List<LongRange> { new LongRange("3-9"), new LongRange("12-20") });

            // assert
            Assert.That(foo1.ToString(), Is.EqualTo("1-10,12-20"));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<IntegerRange> { new IntegerRange("3-6"), new IntegerRange("11"), new IntegerRange("35") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-6,8,10-15,19,35"));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<IntegerRange> { new IntegerRange("3-11"), new IntegerRange("13-14"), new IntegerRange("19-21") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19-21"));
        }
    }
}
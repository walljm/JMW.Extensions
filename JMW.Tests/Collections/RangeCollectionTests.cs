using System;
using System.Collections.Generic;
using JMW.Extensions.Enumerable;
using JMW.Types;
using Lambda.Generic.Arithmetic;
using NUnit.Framework;

namespace JMW.Collections.Tests
{
    [TestFixture]
    public class RangeCollectionTests
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
        public void RangeTest2()
        {
            var rng = new IntegerRange(2, 6);
            Assert.AreEqual(false, rng.Contains(1));
            Assert.AreEqual(true, rng.Contains(2));
            Assert.AreEqual(true, rng.Contains(3));
            Assert.AreEqual(true, rng.Contains(4));
            Assert.AreEqual(true, rng.Contains(5));
            Assert.AreEqual(true, rng.Contains(6));
            Assert.AreEqual(false, rng.Contains(7));

            Assert.AreEqual(true, rng.Contains("5"));

            Assert.AreEqual("2-6", rng.ToString());
            Assert.AreEqual("2,3,4,5,6", rng.GetNumbers().ToDelimitedString(','));

            rng = new IntegerRange
            {
                Start = 2,
                Stop = 6
            };
            Assert.AreEqual(true, rng.IntersectsWith("1,5"));
            Assert.AreEqual(true, rng.IntersectsWith("3:5"));
            Assert.AreEqual(true, rng.IntersectsWith("3-8"));

            Assert.AreEqual(true, rng.Contains(new IntegerRange(3, 5)));
            Assert.AreEqual(true, rng.Contains(new IntegerRange(2, 6)));
            Assert.AreEqual(false, rng.Contains(new IntegerRange(3, 8)));
            Assert.AreEqual(false, rng.Contains(new IntegerRange(1, 5)));
        }

        [Test]
        public void RangeTest3()
        {
            var rng = new LongRange(2, 6);
            Assert.AreEqual(false, rng.Contains(1));
            Assert.AreEqual(true, rng.Contains(2));
            Assert.AreEqual(true, rng.Contains(3));
            Assert.AreEqual(true, rng.Contains(4));
            Assert.AreEqual(true, rng.Contains(5));
            Assert.AreEqual(true, rng.Contains(6));
            Assert.AreEqual(false, rng.Contains(7));

            Assert.AreEqual(true, rng.Contains("5"));

            Assert.AreEqual("2-6", rng.ToString());
            Assert.AreEqual("2,3,4,5,6", rng.GetNumbers().ToDelimitedString(','));

            rng = new LongRange
            {
                Start = 2,
                Stop = 6
            };
            Assert.AreEqual(true, rng.IntersectsWith("1,5"));
            Assert.AreEqual(true, rng.IntersectsWith("3:5"));
            Assert.AreEqual(true, rng.IntersectsWith("3-8"));

            var rng1 = new LongRange(rng)
            {
                Stop = 11,
                Start = 6
            };
            Assert.AreEqual(false, rng1.IntersectsWith("1,5"));
            Assert.AreEqual(true, rng1.IntersectsWith("6:8"));
            Assert.AreEqual(true, rng1.IntersectsWith("3-15"));
            Assert.AreEqual(false, rng1.IntersectsWith("12-15"));

            Assert.AreEqual(false, rng.IntersectsWith("0-1"));
            Assert.AreEqual(false, rng.IntersectsWith("7-8"));

            try
            {
                _ = new IntegerRange("3,1");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Stop cannot be less than Start (Parameter 'rng')", ex.Message);
            }
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
            _ = new LongRangeCollection(new List<LongRange>{
                new LongRange(1,5),
                new LongRange(new Range<long, LongMath>(7,10)),
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
            var foo = new IntegerRangeCollection("1-4,10,8, 12-15,19")
            {
                new IntegerRange("3-11")
            };

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19"));
            foo.Clear();
            Assert.AreEqual(0, foo.Count);

            var foo1 = new LongRangeCollection("1-4,10,8, 12-15,19");
            foo1.AddRange(new List<LongRange> { new LongRange("3-9"), new LongRange("12-20") });

            // assert
            Assert.That(foo1.ToString(), Is.EqualTo("1-10,12-20"));
            Assert.IsTrue(foo1.Contains("1"));
            Assert.IsTrue(foo1.Contains("2"));
            Assert.IsTrue(foo1.Contains("3"));
            Assert.IsTrue(foo1.Contains("4"));
            Assert.IsTrue(foo1.Contains("5"));
            Assert.IsTrue(foo1.Contains("6"));
            Assert.IsTrue(foo1.Contains("7"));
            Assert.IsTrue(foo1.Contains("8"));
            Assert.IsTrue(foo1.Contains("9"));
            Assert.IsTrue(foo1.Contains("10"));
            Assert.IsFalse(foo1.Contains("11"));
            Assert.IsTrue(foo1.Contains("12"));
            Assert.IsTrue(foo1.Contains("13"));
            Assert.IsTrue(foo1.Contains("14"));
            Assert.IsTrue(foo1.Contains("15"));
            Assert.IsTrue(foo1.Contains("16"));
            Assert.IsTrue(foo1.Contains("17"));
            Assert.IsTrue(foo1.Contains("18"));
            Assert.IsTrue(foo1.Contains("19"));
            Assert.IsTrue(foo1.Contains("20"));
            Assert.IsFalse(foo1.Contains("21"));
            Assert.AreEqual("1,2,3,4,5,6,7,8,9,10,12,13,14,15,16,17,18,19,20", foo1.GetNumbers().ToDelimitedString(","));
            Assert.AreEqual("1,2,3,4,5,6,7,8,9,10,12,13,14,15,16,17,18,19,20", IntegerRangeCollection.ExplodeRange("1-10,12-20").ToDelimitedString(","));
            Assert.AreEqual(true, foo1.IntersectsWith(new LongRange(0, 8)));
            Assert.AreEqual(true, foo1.IntersectsWith(new LongRange(0, 30)));
            Assert.AreEqual(true, foo1.IntersectsWith(new LongRange(17, 30)));
            Assert.AreEqual(true, foo1.IntersectsWith(new LongRange(0, 5)));
            Assert.AreEqual(true, foo1.IntersectsWith(new LongRange(5, 15)));
            Assert.AreEqual(false, foo1.IntersectsWith(new LongRange(21, 22)));
            Assert.AreEqual(false, foo1.IntersectsWith(new LongRange(-3, 0)));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<string>{"3-6","11","35"});

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-6,8,10-15,19,35"));

            foo = new IntegerRangeCollection("1-4,10,8, 12-15,19");
            foo.AddRange(new List<IntegerRange> { new IntegerRange("3-11"), new IntegerRange("13-14"), new IntegerRange("19-21") });

            // assert
            Assert.That(foo.ToString(), Is.EqualTo("1-15,19-21"));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace JMW.Extensions.Enumerable.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void ReverseOrderTest()
        {
            CollectionAssert.AreEqual(new List<int> { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.ReverseOrder().ToList());
        }

        [Test]
        public void LastTest1()
        {
            var lst = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            CollectionAssert.AreEqual(new List<int> { 6, 7, 8, 9, 10 }, lst.Last(5));
            CollectionAssert.AreEqual(new List<int> { 8, 9, 10 }, lst.Last(3));
            Assert.AreEqual(0, lst.Last(-3).Count());
            Assert.AreEqual(0, lst.Last(0).Count());
            CollectionAssert.AreEqual(lst, lst.Last(30));
        }

        [Test]
        public void ToHashSetTest1()
        {
            var lst = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var hsh = lst.ToHashSet();

            Assert.AreEqual(10, hsh.Count);
            Assert.AreEqual(typeof(HashSet<int>), hsh.GetType());
        }

        [Test]
        public void MergeTest1()
        {
            var lst1 = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var lst2 = new HashSet<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var lst = lst1.Merge(lst2);

            Assert.AreEqual(15, lst.Count);
            lst.Merge(11);
            Assert.AreEqual(15, lst.Count);
            lst.Merge(16);
            Assert.AreEqual(16, lst.Count);

            lst.Merge(lst2.ToList());
            Assert.AreEqual(16, lst.Count);
            lst.Merge(new List<int> { 3, 5, 18, 19 });
            Assert.AreEqual(18, lst.Count);
        }

        [Test]
        public void CreateHistogramTest()
        {
            var lst1 = new List<int> { 1, 1, 1, 2, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 7, 8, 9, 9, 10 };

            var lst = lst1.CreateHistogram(i => i.ToString());

            Assert.AreEqual(10, lst.Count);
            Assert.AreEqual(3, lst["1"].Count);
            Assert.AreEqual(1, lst["2"].Count);
            Assert.AreEqual(2, lst["3"].Count);
            Assert.AreEqual(4, lst["4"].Count);
            Assert.AreEqual(3, lst["5"].Count);
            Assert.AreEqual(2, lst["6"].Count);
            Assert.AreEqual(1, lst["7"].Count);
            Assert.AreEqual(1, lst["8"].Count);
            Assert.AreEqual(2, lst["9"].Count);
            Assert.AreEqual(1, lst["10"].Count);

            var lst2 = lst1.CreateHistogram(i => i);

            Assert.AreEqual(10, lst2.Count);
            Assert.AreEqual(3, lst2[1].Count);
            Assert.AreEqual(1, lst2[2].Count);
            Assert.AreEqual(2, lst2[3].Count);
            Assert.AreEqual(4, lst2[4].Count);
            Assert.AreEqual(3, lst2[5].Count);
            Assert.AreEqual(2, lst2[6].Count);
            Assert.AreEqual(1, lst2[7].Count);
            Assert.AreEqual(1, lst2[8].Count);
            Assert.AreEqual(2, lst2[9].Count);
            Assert.AreEqual(1, lst2[10].Count);
        }

        [Test]
        public void ToDelimitedStringTest()
        {
            var lst = new HashSet<int> { 1, 2, 3, 4 };
            Assert.AreEqual("1,2,3,4", lst.ToDelimitedString(','));
            Assert.AreEqual("1,2,3,4,", lst.ToDelimitedString(',', false));

            Assert.AreEqual("1,,2,,3,,4", lst.ToDelimitedString(",,"));
            Assert.AreEqual("1,,2,,3,,4,,", lst.ToDelimitedString(",,", false));
        }

        [Test]
        public void IslastTest()
        {
            Assert.AreEqual(true, new List<int> { 1, 2, 3, 4 }.IsLast(3));
            Assert.AreEqual(true, new[] { 1, 2, 3, 4 }.IsLast(3));
            Assert.AreEqual(true, new[] { 1, 2, 3, 4 }.Select(i => i).IsLast(3));

            Assert.AreEqual(false, new List<int> { 1, 2, 3, 4 }.Select(i => i).IsLast(2));
            Assert.AreEqual(false, new List<int> { 1, 2, 3, 4 }.IsLast(2));
            Assert.AreEqual(false, new[] { 1, 2, 3, 4 }.IsLast(2));
        }

        [Test]
        public void ToListOfItemTest()
        {
            var lst = 1.ToListOfItem();
            Assert.AreEqual(1, lst.Count);
        }

        [Test]
        public void YieldTest()
        {
            var lst = 1.Yield();
            Assert.AreEqual(1, lst.Count());
        }

        [Test]
        public void ShimTest()
        {
            var lst = new List<object> { 1 }.Shim<int>();
            Assert.AreEqual(1, lst.Count());
        }

        [Test]
        public void GetRangeSafeTest()
        {
            var lst = new List<int> { 1, 2, 3, 4 };
            Assert.That(new List<int> { 2, 3 }, Is.EquivalentTo(lst.GetRangeSafe(1, 2)));
            Assert.That(new List<int> { 2, 3, 4 }, Is.EquivalentTo(lst.GetRangeSafe(1, 3)));
            Assert.That(new List<int> { 2, 3, 4 }, Is.EquivalentTo(lst.GetRangeSafe(1, 5)));
            Assert.That(new List<int>(), Is.EquivalentTo(lst.GetRangeSafe(5, 2)));
        }

        [Test]
        public void EachTest()
        {
            var lst = new List<Foo> { new Foo { Value = 1 }, new Foo { Value = 2 }, new Foo { Value = 3 }, new Foo { Value = 4 } };
            lst.Each(i => i.Value = i.Value * 2);

            Assert.That(new List<int> { 2, 4, 6, 8 }, Is.EquivalentTo(lst.Select(i => i.Value)));
        }

        [Test]
        public void GroupIntoSetsTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0 },
                new Foo { Value = 1 },
                new Foo { Value = 2 },
                new Foo { Value = 3 },
                new Foo { Value = 4 },
                new Foo { Value = 5 },
                new Foo { Value = 6 },
                new Foo { Value = 7 },
                new Foo { Value = 8 },
                new Foo { Value = 9 }
            };

            var sets = lst.GroupIntoSets(o => o.Value % 3 == 0).ToList();

            Assert.That(sets.Count, Is.EqualTo(4));
            Assert.That(sets[0].Count, Is.EqualTo(3));
            Assert.That(sets[3].Count, Is.EqualTo(1));
        }

        [Test]
        public void ToDictionaryOfManyTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=30 },
                new Foo { Value = 3, Bar=40 },
                new Foo { Value = 4, Bar=50 },
                new Foo { Value = 5, Bar=60 },
                new Foo { Value = 6, Bar=70 },
                new Foo { Value = 7, Bar=80 },
                new Foo { Value = 8, Bar=90 },
                new Foo { Value = 9, Bar=100 }
            };
            var dict = lst.ToDictionaryOfMany(o => new List<int> { o.Value, o.Bar });

            Assert.That(dict.Count, Is.EqualTo(20));
            Assert.That(dict[0], Is.EqualTo(dict[10]));
        }

        [Test]
        public void ToDictionaryOfHashSetsTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=10 },
                new Foo { Value = 3, Bar=20 },
                new Foo { Value = 4, Bar=10 },
                new Foo { Value = 5, Bar=20 },
                new Foo { Value = 6, Bar=10 },
                new Foo { Value = 7, Bar=20 },
                new Foo { Value = 8, Bar=10 },
                new Foo { Value = 9, Bar=20 }
            };
            var dict = lst.ToDictionaryOfHashSets(o => o.Bar);

            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict[10].Count, Is.EqualTo(5));
        }

        [Test]
        public void ToDictionaryOfListsTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=10 },
                new Foo { Value = 3, Bar=20 },
                new Foo { Value = 4, Bar=10 },
                new Foo { Value = 5, Bar=20 },
                new Foo { Value = 6, Bar=10 },
                new Foo { Value = 7, Bar=20 },
                new Foo { Value = 8, Bar=10 },
                new Foo { Value = 9, Bar=20 }
            };
            var dict = lst.ToDictionaryOfLists(o => o.Bar);

            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict[10].Count, Is.EqualTo(5));
        }

        [Test]
        public void ToDictionaryOfListsOfManyTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=10 },
                new Foo { Value = 3, Bar=20 },
                new Foo { Value = 4, Bar=10 },
                new Foo { Value = 5, Bar=20 },
                new Foo { Value = 6, Bar=10 },
                new Foo { Value = 7, Bar=20 },
                new Foo { Value = 8, Bar=10 },
                new Foo { Value = 9, Bar=20 }
            };
            var dict = lst.ToDictionaryOfListsOfMany(o => new List<int> { o.Bar });

            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict[10].Count, Is.EqualTo(5));
        }

        [Test]
        public void ToDictionaryOfHashSetsOfManyTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=10 },
                new Foo { Value = 3, Bar=20 },
                new Foo { Value = 4, Bar=10 },
                new Foo { Value = 5, Bar=20 },
                new Foo { Value = 6, Bar=10 },
                new Foo { Value = 7, Bar=20 },
                new Foo { Value = 8, Bar=10 },
                new Foo { Value = 9, Bar=20 }
            };
            var dict = lst.ToDictionaryOfHashSetsOfMany(o => new List<int> { o.Bar, o.Value });

            Assert.That(dict.Count, Is.EqualTo(12));
            Assert.That(dict[10].Count, Is.EqualTo(5));
        }

        
        [Test]
        public void GetDuplicatesTest()
        {
            var lst = new List<int>
            {
                1,1,2,2,3,4,5,6,7,8,9,0,0,2,4,3,2,6
            };
            var dict = lst.GetDuplicates().ToList();

            Assert.That(dict.Count, Is.EqualTo(8));
        }

        
        [Test]
        public void DisctinctTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=10 },
                new Foo { Value = 3, Bar=20 },
                new Foo { Value = 4, Bar=10 },
                new Foo { Value = 5, Bar=20 },
                new Foo { Value = 6, Bar=10 },
                new Foo { Value = 7, Bar=20 },
                new Foo { Value = 8, Bar=10 },
                new Foo { Value = 9, Bar=20 }
            };
            var dict = lst.Distinct(k => k.Bar).ToList();

            Assert.That(dict.Count, Is.EqualTo(2));
        }
        
        [Test]
        public void ShuffleTest()
        {
            var lst = new List<int>
            {
                1,2,3,4,5,6,7,8,9,0
            };
            lst.Shuffle();

            Assert.That(lst[0] != 1);
        }
        
        [Test]
        public void IfHasKeyTest()
        {
            var lst = new List<Foo>
            {
                new Foo { Value = 0, Bar=10 },
                new Foo { Value = 1, Bar=20 },
                new Foo { Value = 2, Bar=30 },
                new Foo { Value = 3, Bar=40 },
                new Foo { Value = 4, Bar=50 },
                new Foo { Value = 5, Bar=60 },
                new Foo { Value = 6, Bar=70 },
                new Foo { Value = 7, Bar=80 },
                new Foo { Value = 8, Bar=90 },
                new Foo { Value = 9, Bar=100 }
            };
            var dict = lst.ToDictionaryOfMany(o => new List<int> { o.Value, o.Bar });

            dict.IfHasKey(0, o =>
            {
                Assert.AreEqual(0, o.Value);
                Assert.AreEqual(10, o.Bar);
            });
            dict.IfHasKey(99, o => { Assert.Fail(); });

            var did = dict.TryAdd(0, new Foo());
            Assert.IsFalse(did);
        }

        private class Foo
        {
            public int Value { get; set; } = 0;
            public int Bar { get; set; } = 0;
        }
    }
}
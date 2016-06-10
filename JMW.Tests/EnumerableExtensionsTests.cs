using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace JMW.Extensions.Enumerable.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void LastTest1()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var t1 = lst.Last(-3);

            CollectionAssert.AreEqual(new List<int>() { 6, 7, 8, 9, 10 }, lst.Last(5));
            CollectionAssert.AreEqual(new List<int>() { 8, 9, 10 }, lst.Last(3));
            Assert.AreEqual(0, lst.Last(-3).Count());
            Assert.AreEqual(0, lst.Last(0).Count());
            CollectionAssert.AreEqual(lst, lst.Last(30));
        }

        [Test]
        public void ToHashSetTest1()
        {
            var lst = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var hsh = lst.ToHashSet();

            Assert.AreEqual(10, hsh.Count);
            Assert.AreEqual(typeof(HashSet<int>), hsh.GetType());
        }

        [Test]
        public void MergeTest1()
        {
            var lst1 = new HashSet<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var lst2 = new HashSet<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var lst = lst1.Merge(lst2);

            Assert.AreEqual(15, lst.Count);
        }
    }
}
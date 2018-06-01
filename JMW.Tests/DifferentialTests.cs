using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace JMW.Differentials.Tests
{
    [TestFixture]
    public class DifferentialTests
    {
        [Test]
        public void CollectionDifferential1()
        {
            var lstA = new List<string> { "b", "c", "d" };
            var lstB = new List<string> { "A", "B", "c" };

            var diff = Differential.RunCollectionDifferential(
                lstA,
                lstB,
                (k, lst) =>
                {
                    return lst.FirstOrDefault(i => i.ToLower() == k.ToLower());
                },
                (a, b) =>
                {
                    if (a != b)
                    {
                        return new ObjectDifferential<string, MyDiff>(a, b, new List<MyDiff> { new MyDiff { Difference = a + " " + b } });
                    }

                    return null;
                });

            Assert.AreEqual(1, diff.OnlyInListA.Count);
            Assert.AreEqual(1, diff.OnlyInListB.Count);
            Assert.AreEqual(1, diff.Modified.Count);
            Assert.AreEqual("b B", diff.Modified.First().Differences.First().Difference);
            Assert.AreEqual("b", diff.Modified.First().ObjectA);
            Assert.AreEqual("B", diff.Modified.First().ObjectB);
        }

        [Test]
        public void PairTest1()
        {
            var o1 = "Jason";
            var o2 = "Wall";
            var o3 = "Jason";
            var o4 = "Wall";

            var p1 = new Pair<string>(o1, o2);
            var p2 = new Pair<string>(o3, o4);

            Assert.AreEqual(p1, p2);
            Assert.IsTrue(p1.Equals(p2));
            Assert.IsTrue(p1.Equals((object)p2));
            Assert.IsTrue(p1.Equals(p1, p2));

            Assert.IsTrue(((IStructuralEquatable)p1).Equals(p2));
            Assert.IsTrue(p1.Equals((object)p2));

            Assert.IsTrue(((IStructuralEquatable)p1).Equals(p2, EqualityComparer<string>.Default));
            Assert.IsTrue(p1.Equals((object)p2));


            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1 == p2);

            Assert.AreEqual(0, p1.CompareTo(p2));

            o1 = "1Jason";
            p1 = new Pair<string>(o1, o2);
            Assert.AreEqual(-1, p1.CompareTo(p2));
            o1 = "Jason1";
            p1 = new Pair<string>(o1, o2);
            Assert.AreEqual(1, p1.CompareTo(p2));
            Assert.AreEqual(1, p1.CompareTo(null));

            Assert.AreEqual(1, ((IComparable)p1).CompareTo(p2));
            Assert.AreEqual(1, ((IComparable)p1).CompareTo(null));


            o1 = "Jason";
            o2 = "Wall";
            o3 = "Jason";
            o4 = "Wall1";

            p1 = new Pair<string>(o1, o2);
            p2 = new Pair<string>(o3, o4);

            Assert.AreNotEqual(p1, p2);
            Assert.IsFalse(p1.Equals(p2));
            Assert.IsFalse(p1.Equals(null));
            Assert.IsFalse(null == p1);
            Assert.IsFalse(p1 == null);
            Assert.IsFalse((Pair<string>)null != null);
            Assert.IsTrue(p1 !=p2);
            Assert.IsTrue(p1 != null);
            Assert.IsTrue(null != p2);

        }

        [Test]
        public void PairTest2()
        {
            var o1 = "Jason";
            var o2 = 1;
            var o3 = "Jason";
            var o4 = 1;

            var p1 = new Pair<string, int>(o1, o2);
            var p2 = new Pair<string, int>(o3, o4);

            Assert.AreEqual(p1, p2);


            Assert.AreEqual(p1, p2);
            Assert.IsTrue(p1.Equals(p2));
            Assert.IsTrue(p1.Equals((object)p2));
            Assert.IsTrue(p1.Equals(p1, p2));

            Assert.IsTrue(((IStructuralEquatable)p1).Equals(p2));
            Assert.IsTrue(p1.Equals((object)p2));

            Assert.IsTrue(((IStructuralEquatable)p1).Equals(p2, EqualityComparer<string>.Default));
            Assert.IsTrue(p1.Equals((object)p2));


            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1 == p2);
            Assert.IsTrue(p1 == p2);

            Assert.AreEqual(0, p1.CompareTo(p2));

            o1 = "1Jason";
            p1 = new Pair<string, int>(o1, o2);
            Assert.AreEqual(-1, p1.CompareTo(p2));
            o1 = "Jason1";
            p1 = new Pair<string, int>(o1, o2);
            Assert.AreEqual(1, p1.CompareTo(p2));
            Assert.AreEqual(1, p1.CompareTo(null));

            Assert.AreEqual(1, ((IComparable)p1).CompareTo(p2));
            Assert.AreEqual(1, ((IComparable)p1).CompareTo(null));
            Assert.AreEqual(0, new PairComparer<string, int>().Compare(null, null));
            Assert.AreEqual(1, new PairComparer<string, int>().Compare(null, p2));


            o1 = "Jason";
            o2 = 1;
            o3 = "Jason";
            o4 = 2;

            p1 = new Pair<string, int>(o1, o2);
            p2 = new Pair<string, int>(o3, o4);

            Assert.AreNotEqual(p1, p2);
            Assert.IsFalse(p1.Equals(p2));
            Assert.IsFalse(p1.Equals(null));
            Assert.IsFalse(null == p1);
            Assert.IsFalse(p1 == null);
            Assert.IsFalse((Pair<string>)null != null);
            Assert.IsTrue(p1 != p2);
            Assert.IsTrue(p1 != null);
            Assert.IsTrue(null != p2);
        }

        private class MyDiff
        {
            public string Difference { get; set; } = string.Empty;
        }
    }

    [TestFixture]
    public class DifferTests
    {
        [Test]
        public void RunCollectionDifferential_Selector_test()
        {
            var lst_a = new List<T1>
            {
                new T1 { Foo="1"},
                new T1 { Foo="2"},
                new T1 { Foo="3"},
                new T1 { Foo="4"}
            };
            var lst_b = new List<T1>
            {
                new T1 { Foo="2"},
                new T1 { Foo="4"},
                new T1 { Foo="5"},
                new T1 { Foo="6"}
            };

            var diff = Differential.RunCollectionDifferential(lst_a, lst_b, p => p.Foo);
            Assert.AreEqual(2, diff.OnlyInListA.Count);
            Assert.AreEqual(2, diff.OnlyInListB.Count);
            Assert.AreEqual(2, diff.Shared.Count);
        }

        [Test]
        public void RunCollectionDifferential_Selector_Two_Types_test()
        {
            var lst_a = new List<T1>
            {
                new T1 { Foo="1"},
                new T1 { Foo="2"},
                new T1 { Foo="3"},
                new T1 { Foo="4"}
            };
            var lst_b = new List<T2>
            {
                new T2 { Foo="2"},
                new T2 { Foo="4"},
                new T2 { Foo="5"},
                new T2 { Foo="6"}
            };

            var diff = Differential.RunCollectionDifferential(lst_a, lst_b, p => p.Foo, p => p.Foo);
            Assert.AreEqual(2, diff.OnlyInListA.Count);
            Assert.AreEqual(2, diff.OnlyInListB.Count);
            Assert.AreEqual(2, diff.Shared.Count);
        }
    }

    internal class StringComparer : IComparer<string>
    {
        #region Implementation of IComparer<in string>

        public int Compare(string x, string y)
        {
            return String.Compare(x, y, StringComparison.Ordinal);
        }

        #endregion Implementation of IComparer<in string>
    }

    internal class T1
    {
        public string Foo { get; set; } = string.Empty;

        public int Bar { get; set; } = -1;

        public List<string> Jason { get; set; } = new List<string>();
    }

    internal class T2
    {
        public string Foo { get; set; } = string.Empty;

        public List<int> Jason { get; set; } = new List<int>();

        public bool Wall { get; set; } = false;
    }
}
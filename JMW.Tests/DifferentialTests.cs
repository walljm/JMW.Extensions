using System;
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

            var diff = Differerential.RunCollectionDifferential(
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

            var diff = Differerential.RunCollectionDifferential(lst_a, lst_b, p => p.Foo);
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

            var diff = Differerential.RunCollectionDifferential(lst_a, lst_b, p => p.Foo, p => p.Foo);
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
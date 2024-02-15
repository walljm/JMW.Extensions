using JMW.Types;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                    return lst.FirstOrDefault(i => i.Equals(k, StringComparison.OrdinalIgnoreCase));
                },
                (a, b) =>
                {
                    return a != b ? new ObjectDifferential<string, MyDiff>(a, b, [new MyDiff { Difference = a + " " + b }]) : null;
                });

            Assert.That(1, Is.EqualTo(diff.OnlyInListA.Count));
            Assert.That(1, Is.EqualTo(diff.OnlyInListB.Count));
            Assert.That(1, Is.EqualTo(diff.Modified.Count));
            Assert.That("b B", Is.EqualTo(diff.Modified.First().Differences.First().Difference));
            Assert.That("b", Is.EqualTo(diff.Modified.First().ObjectA));
            Assert.That("B", Is.EqualTo(diff.Modified.First().ObjectB));
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

            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1.Equals(p2), Is.True);
            Assert.That(p1.Equals((object)p2), Is.True);
            Assert.That(p1.Equals(p1, p2), Is.True);

            Assert.That(((IStructuralEquatable)p1).Equals(p2), Is.True);
            Assert.That(p1.Equals((object)p2), Is.True);

            Assert.That(((IStructuralEquatable)p1).Equals(p2, EqualityComparer<string>.Default), Is.True);
            Assert.That(p1.Equals((object)p2), Is.True);

            Assert.That(p1 == p2, Is.True);
            Assert.That(p1 == p2, Is.True);
            Assert.That(p1 == p2, Is.True);
            Assert.That(p1 == p2, Is.True);

            Assert.That(0, Is.EqualTo(p1.CompareTo(p2)));

            o1 = "1Jason";
            p1 = new Pair<string>(o1, o2);
            Assert.That(-1, Is.EqualTo(p1.CompareTo(p2)));
            o1 = "Jason1";
            p1 = new Pair<string>(o1, o2);
            Assert.That(1, Is.EqualTo(p1.CompareTo(p2)));
            Assert.That(1, Is.EqualTo(p1.CompareTo(null)));

            Assert.That(1, Is.EqualTo(((IComparable)p1).CompareTo(p2)));
            Assert.That(1, Is.EqualTo(((IComparable)p1).CompareTo(null)));

            o1 = "Jason";
            o2 = "Wall";
            o3 = "Jason";
            o4 = "Wall1";

            p1 = new Pair<string>(o1, o2);
            p2 = new Pair<string>(o3, o4);

            Assert.That(p1, Is.Not.EqualTo(p2));
            Assert.That(p1.Equals(p2), Is.False);
            Assert.That(p1.Equals(null), Is.False);
            Assert.That(null == p1, Is.False);
            Assert.That(p1 is null, Is.False);
            Assert.That(p1 != p2, Is.True);
            Assert.That(p1 is not null, Is.True);
            Assert.That(null != p2, Is.True);
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

            Assert.That(p1, Is.EqualTo(p2));

            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1.Equals(p2), Is.True);
            Assert.That(p1.Equals((object)p2), Is.True);
            Assert.That(p1.Equals(p1, p2), Is.True);

            Assert.That(((IStructuralEquatable)p1).Equals(p2), Is.True);
            Assert.That(p1.Equals((object)p2), Is.True);

            Assert.That(((IStructuralEquatable)p1).Equals(p2, EqualityComparer<string>.Default), Is.True);
            Assert.That(p1.Equals((object)p2), Is.True);

            Assert.That(p1 == p2, Is.True);
            Assert.That(p1 == p2, Is.True);
            Assert.That(p1 == p2, Is.True);
            Assert.That(p1 == p2, Is.True);

            Assert.That(0, Is.EqualTo(p1.CompareTo(p2)));

            o1 = "1Jason";
            p1 = new Pair<string, int>(o1, o2);
            Assert.That(-1, Is.EqualTo(p1.CompareTo(p2)));
            o1 = "Jason1";
            p1 = new Pair<string, int>(o1, o2);
            Assert.That(1, Is.EqualTo(p1.CompareTo(p2)));
            Assert.That(1, Is.EqualTo(p1.CompareTo(null)));

            Assert.That(1, Is.EqualTo(((IComparable)p1).CompareTo(p2)));
            Assert.That(1, Is.EqualTo(((IComparable)p1).CompareTo(null)));
            Assert.That(0, Is.EqualTo(new PairComparer<string, int>().Compare(null, null)));
            Assert.That(1, Is.EqualTo(new PairComparer<string, int>().Compare(null, p2)));

            o1 = "Jason";
            o2 = 1;
            o3 = "Jason";
            o4 = 2;

            p1 = new Pair<string, int>(o1, o2);
            p2 = new Pair<string, int>(o3, o4);

            Assert.That(p1, Is.Not.EqualTo(p2));
            Assert.That(p1.Equals(p2), Is.False);
            Assert.That(p1.Equals(null), Is.False);
            Assert.That(null == p1, Is.False);
            Assert.That(p1 is null, Is.False);
            Assert.That(p1 != p2, Is.True);
            Assert.That(p1 is not null, Is.True);
            Assert.That(null != p2, Is.True);
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
                new() { Foo="1"},
                new() { Foo="2"},
                new() { Foo="3"},
                new() { Foo="4"}
            };
            var lst_b = new List<T1>
            {
                new() { Foo="2"},
                new() { Foo="4"},
                new() { Foo="5"},
                new() { Foo="6"}
            };

            var diff = Differential.RunCollectionDifferential(lst_a, lst_b, p => p.Foo);
            Assert.That(2, Is.EqualTo(diff.OnlyInListA.Count));
            Assert.That(2, Is.EqualTo(diff.OnlyInListB.Count));
            Assert.That(2, Is.EqualTo(diff.Shared.Count));
        }

        [Test]
        public void RunCollectionDifferential_Selector_Two_Types_test()
        {
            var lst_a = new List<T1>
            {
                new() { Foo="1"},
                new() { Foo="2"},
                new() { Foo="3"},
                new() { Foo="4"}
            };
            var lst_b = new List<T2>
            {
                new() { Foo="2"},
                new() { Foo="4"},
                new() { Foo="5"},
                new() { Foo="6"}
            };

            var diff = Differential.RunCollectionDifferential(lst_a, lst_b, p => p.Foo, p => p.Foo);
            Assert.That(2, Is.EqualTo(diff.OnlyInListA.Count));
            Assert.That(2, Is.EqualTo(diff.OnlyInListB.Count));
            Assert.That(2, Is.EqualTo(diff.Shared.Count));
        }
    }

    internal class StringComparer : IComparer<string>
    {
        #region Implementation of IComparer<in string>

        public int Compare(string? x, string? y)
        {
            return string.Compare(x, y, StringComparison.Ordinal);
        }

        #endregion Implementation of IComparer<in string>
    }

    internal class T1
    {
        public string Foo { get; set; } = string.Empty;

        public int Bar { get; set; } = -1;

        public List<string> Jason { get; set; } = [];
    }

    internal class T2
    {
        public string Foo { get; set; } = string.Empty;

        public List<int> Jason { get; set; } = [];

        public bool Wall { get; set; } = false;
    }
}
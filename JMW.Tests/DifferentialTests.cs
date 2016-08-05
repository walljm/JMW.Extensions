using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using JMW.Differentials;

namespace JMW.Types.Functional.Tests
{
    [TestFixture]
    public class DifferentialTests
    {
        [Test]
        public void CollectionDifferential1()
        {
            var lstA = new List<string> { "b", "c", "d" };
            var lstB = new List<string> { "A", "B", "c" };

            var diff = Differerential.DiffList<string, MyDiff>(
                lstA, 
                lstB, 
                (k, lst) => {
                    return lst.FirstOrDefault(i => i.ToLower() == k.ToLower());
                }, 
                (a, b) => {
                    if (a != b)
                    {
                        return new ObjectDifferential<string, MyDiff>(a, b, new List<MyDiff>() { new MyDiff() { Difference = a + " " + b } });
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
            public string Difference { get; set; } = "";
        }

    }
}
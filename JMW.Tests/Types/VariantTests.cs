using System.Collections.Generic;
using NUnit.Framework;

namespace JMW.Types.Tests
{
    [TestFixture]
    public class VariantTests
    {
        [Test]
        public void VariantTest1()
        {
            var v1 = new Variant("jason");
            Assert.AreEqual(true, v1.IsString);
            Assert.AreEqual("jason", v1.StringValue);
            Assert.IsTrue("jason" == (string)v1);

            Variant tv1 = "jason";
            Assert.AreEqual(true, tv1.IsString);
            Assert.AreEqual("jason", tv1.StringValue);
            v1.StringValue = "";
            Assert.AreEqual("", v1.StringValue);
            Assert.IsTrue(v1.GetHashCode() > 0);

            var v2 = new Variant(new List<Variant> { "jason" });
            Assert.AreEqual(true, v2.IsList);
            Assert.AreEqual(new List<Variant> { "jason" }, v2.ListValue);
            
            Variant tv2 = new List<Variant> { "jason" };
            Assert.AreEqual(true, tv2.IsList);
            Assert.AreEqual(new List<Variant> { "jason" }, tv2.ListValue);
            Assert.IsTrue(tv2.Equals(new List<Variant> { "jason" }));
            Assert.IsFalse(tv2.Equals(null));
            Assert.IsTrue(((Variant)tv2).IsList);
            Assert.IsTrue(((Variant)new List<string>{"jason"}).IsList);
            Assert.AreEqual(1, ((List<Variant>) tv2).Count);
            Assert.AreEqual(1, ((List<string>)tv2).Count);
            v2.ListValue = new List<Variant>();
            Assert.AreEqual(0, v2.ListValue.Count);
            Assert.AreEqual("(List) Count: 1", tv2.ToString());
            Assert.IsTrue(v2.GetHashCode() > 0);

            var v3 = new Variant(new Dictionary<string, Variant> { { "one", "jason" } });
            Assert.AreEqual(true, v3.IsDict);
            Assert.AreEqual(new Dictionary<string, Variant> { { "one", "jason" } }, v3.DictValue);

            Variant tv3 = new Dictionary<string, Variant> { { "one", "jason" } };
            Assert.AreEqual(true, tv3.IsDict);
            Assert.AreEqual(new Dictionary<string, Variant> { { "one", "jason" } }, tv3.DictValue);
            Assert.IsTrue(tv3.Equals(new Dictionary<string, Variant> { { "one", "jason" } }));
            Assert.IsTrue(((Variant)new Dictionary<string, Variant> { {"one", "jason" }}).IsDict);
            Assert.AreEqual(1, ((Dictionary<string, Variant>)tv3).Count);
            v3.DictValue = new Dictionary<string, Variant>();
            Assert.AreEqual(0, v3.DictValue.Count);
            Assert.AreEqual("(Dict) Count: 1", tv3.ToString());
            Assert.IsTrue(v3.GetHashCode() > 0);

            var v = new Variant();
            Assert.AreEqual("", v.ToString());
        }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JMW.Types.Tests
{
    [TestFixture]
    public class StringEnumTests
    {
        [Test]
        public void StringEnumTest1()
        {
            var val = ExampleEnum.ONE;
            var v2 = ExampleEnum2.ONE;
            var v3 = ExampleEnum2.TWO;

            Assert.AreEqual(val, ExampleEnum.GetValue("One"));
            Assert.AreEqual(val, ExampleEnum.GetValue(0));
            Assert.AreEqual(val, ExampleEnum.GetValue(ExampleEnum.ONE));
            Assert.That(val.Equals(ExampleEnum.ONE));
            Assert.That(!val.Equals(ExampleEnum.TWO));
            Assert.That(val.Equals("One"));
            Assert.That(val == 0);
            Assert.That(null == (ExampleEnum)3);
            Assert.That(ExampleEnum.ONE == 0);
            Assert.That(ExampleEnum.TWO == 1);
            Assert.That(!val.Equals("Two"));
            Assert.That(val == "One");
            Assert.That(val != "Two");
            Assert.That(val != 1);
            Assert.That(val.ToString() == "One");
            Assert.That(ExampleEnum.GetValue("blah") == (string)null);
            Assert.That(ExampleEnum.GetValue(13) == (string)null);
            Assert.That(val.Equals((object)ExampleEnum.ONE));
            Assert.That(!val.Equals((string)null));
            Assert.IsFalse(val == (string)null);
            string t1 = null;
            ExampleEnum t2 = null;

            Assert.IsFalse(val == t1);
            Assert.IsFalse(val == t2);
            // ReSharper disable once EqualExpressionComparison
            Assert.That(t2 == t2);
            Assert.IsFalse(t2 == val);
            Assert.IsFalse(t2 == "One");

            Assert.IsTrue(val != t1);
            Assert.IsTrue(val != t2);
            Assert.IsTrue(t2 != val);
            Assert.IsTrue(t2 != "One");

            Assert.That(val.CompareTo(ExampleEnum.TWO) < 0);
            Assert.That(val.CompareTo((object)ExampleEnum.ONE) == 0);
            Assert.That(val.CompareTo("Two") < 0);
            Assert.That(val.CompareTo((string)null) == 1);
            Assert.That(val.CompareTo(t2) == 1);
            Assert.That(ExampleEnum.GetValue(t1) == (string)null);
            Assert.That(ExampleEnum.GetValue(t2) == (string)null);

            var vals = ExampleEnum.GetValues();
            var vals2 = ExampleEnum2.GetValues();

            CollectionAssert.AreEqual(new List<string>() { "One", "Two" }, vals);

            Assert.IsTrue((ExampleEnum)null == (string)null);
            Assert.IsFalse((ExampleEnum)null == 1);

            Assert.IsFalse(val.Equals((object)null));
            Assert.IsFalse(val.Equals((object)1));
            Assert.IsFalse(val.Equals((object)"blah"));
        }

        [Test]
        public void StringEnumTest2()
        {
            foo f = new foo();
            f.E1 = ExampleEnum.TWO;

            //f.E1 = "One";

            var s1 = new JsonSerializerSettings();
            s1.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            s1.Converters.Add(new Newtonsoft.Json.Converters.KeyValuePairConverter());
            s1.NullValueHandling = NullValueHandling.Ignore;
            s1.ObjectCreationHandling = ObjectCreationHandling.Replace;
            var json = JsonConvert.SerializeObject(f, s1);

            var s2 = new JsonSerializerSettings();
            s2.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            s2.NullValueHandling = NullValueHandling.Ignore;
            s2.ObjectCreationHandling = ObjectCreationHandling.Replace;
            var f2 = JsonConvert.DeserializeObject<foo>(json, s2);
            Assert.That(f2.E1 == f.E1);

            json = JsonConvert.SerializeObject(new bar1() { E1 = "Two" }, s1);
            f2 = JsonConvert.DeserializeObject<foo>(json, s2);
            Assert.That(f2.E1 == f.E1);

            json = JsonConvert.SerializeObject(new bar2() { E1 = 1 }, s1);
            f2 = JsonConvert.DeserializeObject<foo>(json, s2);
            Assert.That(f2.E1 == f.E1);
        }
    }

    public class bar1
    {
        public string E1 { get; set; } = "One";
    }

    public class bar2
    {
        public int E1 { get; set; } = 0;
    }

    public class foo
    {
        public ExampleEnum E1 { get; set; } = ExampleEnum.ONE;
    }

    public class ExampleEnum : StringEnum<ExampleEnum>
    {
        public ExampleEnum(string value, int ord = -1) : base(value, ord)
        {
        }

        public static ExampleEnum ONE { get; } = new ExampleEnum("One");

        public static ExampleEnum TWO { get; } = new ExampleEnum("Two");

        /// <summary>
        /// Implicitly converts string to ExampleEnum.
        /// </summary>
        /// <param name="value">the string value to convert to a ExampleEnum</param>
        /// <returns>the ExampleEnum with the matching value (case sensitive)</returns>
        public static implicit operator ExampleEnum(string value)
        {
            return (ExampleEnum)GetValue(value);
        }

        /// <summary>
        /// Implicitly converts string to ExampleEnum.
        /// </summary>
        /// <param name="ordinal">the numeric value to convert to a ExampleEnum</param>
        /// <returns>the ExampleEnum with the matching ordinal (case sensitive)</returns>
        public static implicit operator ExampleEnum(long ordinal)
        {
            return (ExampleEnum)GetValue(ordinal);
        }
    }

    public class ExampleEnum2 : StringEnum<ExampleEnum2>
    {
        public ExampleEnum2(string value, int ord = -1) : base(value, ord)
        {
        }

        public static ExampleEnum2 ONE { get; } = new ExampleEnum2("Three");

        public static ExampleEnum2 TWO { get; } = new ExampleEnum2("Four");
    }
}
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

            Assert.AreEqual(val, ExampleEnum.GetValue("One"));
            Assert.AreEqual(val, ExampleEnum.GetValue(0));
            Assert.AreEqual(val, ExampleEnum.GetValue(ExampleEnum.ONE));
            Assert.That(val.Equals(ExampleEnum.ONE));
            Assert.That(!val.Equals(ExampleEnum.TWO));
            Assert.That(val.Equals("One"));
            Assert.That(val == 0);
            Assert.That(null == (ExampleEnum)3);
            Assert.That(null == (ExampleEnum)3);
            Assert.That(ExampleEnum.ONE == 0);
            Assert.That(ExampleEnum.TWO == 1);
            Assert.That(!val.Equals("Two"));
            Assert.That(val == "One");
            Assert.That(val != "Two");
            Assert.That(val.ToString() == "One");
            Assert.That(ExampleEnum.GetValue("blah") == null);
            Assert.That(ExampleEnum.GetValue(new ExampleEnum("jason")) == null);
            Assert.That(val.Equals((object)ExampleEnum.ONE));
            Assert.That(!val.Equals(null));
            Assert.IsFalse(val == null);
            string t1 = null;
            StringEnum t2 = null;

            Assert.IsFalse(val == t1);
            Assert.IsFalse(val == t2);
            Assert.That(t2 == t2);
            Assert.IsFalse(t2 == val);
            Assert.IsFalse(t2 == "One");

            Assert.IsTrue(val != t1);
            Assert.IsTrue(val != t2);
            Assert.IsTrue(t2 != val);
            Assert.IsTrue(t2 != "One");

            Assert.That(val.CompareTo(ExampleEnum.TWO) == -1);
            Assert.That(val.CompareTo((object)ExampleEnum.ONE) == 0);
            Assert.That(val.CompareTo("Two") == -1);
            Assert.That(val.CompareTo(t2) == 1);
            Assert.That(ExampleEnum.GetValue(t1) == null);
            Assert.That(ExampleEnum.GetValue(t2) == null);

            var vals = ExampleEnum.GetValues();

            CollectionAssert.AreEqual(new List<string>() { "One", "Two" }, ExampleEnum.GetValues());

            Assert.IsFalse(val.Equals((object)null));
            Assert.IsFalse(val.Equals((object)1));
            Assert.IsFalse(val.Equals((object)"blah"));
        }

        [Test]
        public void StringEnumTest2()
        {
            foo f = new foo();
            f.E1 = ExampleEnum.TWO;

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
        private string e1 = "One";
        public string E1
        {
            get
            {
                return e1;
            }

            set
            {
                e1 = value;
            }
        }
    }

    public class bar2
    {
        private int e1 = 0;
        public int E1
        {
            get
            {
                return e1;
            }

            set
            {
                e1 = value;
            }
        }
    }

    public class foo
    {
        private ExampleEnum e1 = ExampleEnum.ONE;

        public ExampleEnum E1
        {
            get
            {
                return e1;
            }

            set
            {
                e1 = value;
            }
        }
    }

    public class ExampleEnum : StringEnum
    {
        private const string OneValue = "One";
        private const string TwoValue = "Two";

        static ExampleEnum()
        {
            _One = new ExampleEnum(OneValue);
            _Two = new ExampleEnum(TwoValue, 1);
            ExampleEnum.AddValue(_One);
            ExampleEnum.AddValue(_Two);
        }

        public ExampleEnum(string value, int ord = -1) : base(value, ord)
        {
        }

        private static ExampleEnum _One;
        public static ExampleEnum ONE
        {
            get
            {
                return _One;
            }
        }

        private static ExampleEnum _Two;
        public static ExampleEnum TWO
        {
            get
            {
                return _Two;
            }
        }

        /// <summary>
        /// Explicitly converts string to ExampleEnum.
        /// </summary>
        /// <param name="value">the string value to convert to a ExampleEnum</param>
        /// <returns>the ExampleEnum with the matching value (case sensitive)</returns>
        public static explicit operator ExampleEnum(string value)
        {
            return (ExampleEnum)GetValue(value);
        }

        /// <summary>
        /// Explicitly converts string to ExampleEnum.
        /// </summary>
        /// <param name="ordinal">the numeric value to convert to a ExampleEnum</param>
        /// <returns>the ExampleEnum with the matching ordinal (case sensitive)</returns>
        public static explicit operator ExampleEnum(long ordinal)
        {
            return (ExampleEnum)GetValue(ordinal);
        }
    }
}
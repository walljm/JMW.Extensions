using NUnit.Framework;
using System.Collections.Generic;

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
            Assert.AreEqual(val, ExampleEnum.GetValue(ExampleEnum.ONE));
            Assert.That(val.Equals(ExampleEnum.ONE));
            Assert.That(!val.Equals(ExampleEnum.TWO));
            Assert.That(val.Equals("One"));
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
    }

    public class ExampleEnum : StringEnum
    {
        private const string OneValue = "One";
        private const string TwoValue = "Two";

        static ExampleEnum()
        {
            _One = new ExampleEnum(OneValue);
            _Two = new ExampleEnum(TwoValue);
            ExampleEnum.AddValue(_One);
            ExampleEnum.AddValue(_Two);
        }

        public ExampleEnum(string value) : base(value)
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
    }
}